using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.IntegrationEvents.Transfer;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.Domains.Accounts.Models;
using BankingAppDDD.MongoService.Application.Mongo;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Transfers.Commands
{
    public sealed record TransferFundsCommand(
        Guid accountId,
        Guid destinationAccountId,
        decimal amount,
        string description,
        TransferType transferType = TransferType.SameBank,//this will passed from frontend 
        PaymentGatewayProvider paymentGateway = PaymentGatewayProvider.Internal,
        string? beneficiaryAccountNo = null,
        string? ifscCode = null,
        string? upiId = null,
        string? destinationBankName = null) : Command;

    public sealed class TransferFundsCommandHandler : CommandHandler<TransferFundsCommand>
    {
        private readonly IRepository<Account> _repository;
        private readonly IRepository<BeneficiaryGroup> _beneficiaryRepository;
        private readonly IRepository<FundTransferTransaction> _transferRepository;
        private readonly IAccountMongoService? _mongoService;
        private readonly IBus _eventBus;
        private readonly ILogger<TransferFundsCommandHandler> logger;

        public TransferFundsCommandHandler(
            IRepository<Account> repository,
            IRepository<BeneficiaryGroup> beneficiaryRepository,
            IRepository<FundTransferTransaction> transferRepository,
            IBus eventBus,
            ILogger<TransferFundsCommandHandler> _logger,
            IUnitOfWork unitOfWork,
            IAccountMongoService? mongoService = null) : base(unitOfWork)
        {
            _repository = repository;
            _beneficiaryRepository = beneficiaryRepository;
            _transferRepository = transferRepository;
            _eventBus = eventBus;
            logger = _logger;
            _mongoService = mongoService;
        }

        protected override async Task<bool> HandleAsync(TransferFundsCommand request)
        {
            var correlationId = Guid.NewGuid().ToString();
            logger.LogInformation("Processing fund transfer request. CorrelationId: {CorrelationId}, AccountId: {AccountId}, TransferType: {TransferType}, Amount: {Amount}",
                correlationId, request.accountId, request.transferType, request.amount);

            var originAccount = await _repository.GetByIdAsync(request.accountId);
            if (originAccount is not Account withdrawAccount)
            {
                throw new Exception($"Source Account {request.accountId} not found.");
            }

            // Step 1: Event 1 - Publish FundTransferInitiatedIntegrationEvent (Listened by Fraud Detection, Analytics, Notification Engine)
            var initiatedEvent = new FundTransferInitiatedIntegrationEvent(
                Guid.NewGuid(),
                request.accountId,
                request.destinationAccountId,
                request.amount,
                request.transferType,
                request.paymentGateway,
                request.beneficiaryAccountNo,
                request.ifscCode,
                request.upiId,
                request.destinationBankName,
                request.description ?? "Transfer Initiated",
                correlationId);

            await _eventBus.Publish(initiatedEvent);
            logger.LogInformation("Published FundTransferInitiatedIntegrationEvent for CorrelationId: {CorrelationId}", correlationId);

            // Scenario 1: Same Bank / Internal Transfer to an existing account in our database
            if (request.transferType == TransferType.SameBank && request.destinationAccountId != Guid.Empty)
            {
                var destinationAccount = await _repository.GetByIdAsync(request.destinationAccountId);
                if (destinationAccount is Account depositAccount)
                {
                    // Withdraw from origin account
                    withdrawAccount.Withdraw(request.accountId, request.amount, request.description ?? "Withdraw from source account");
                    _repository.Update(withdrawAccount);

                    // Deposit to beneficiary account
                    depositAccount.Deposit(request.destinationAccountId, request.amount, request.description ?? "Deposited to beneficiary account");
                    _repository.Update(depositAccount);

                    // Store completed transfer record in PostgreSQL
                    var transactionRecord = FundTransferTransaction.Create(
                        request.accountId,
                        request.destinationAccountId,
                        request.amount,
                        TransferType.SameBank,
                        PaymentGatewayProvider.Internal,
                        request.beneficiaryAccountNo,
                        request.ifscCode,
                        request.upiId,
                        request.destinationBankName,
                        request.description ?? "Internal transfer completed",
                        TransferStatus.Completed);

                    _transferRepository.Insert(transactionRecord);
                    await this.UnitOfWork.CommitAsync();

                    // Step 2: Event 2 - Account Debited Event
                    var debitedEvent = new AccountDebitedIntegrationEvent(
                        transactionRecord.TransactionId,
                        request.accountId,
                        request.amount,
                        DateTime.UtcNow,
                        "Account debited for internal transfer",
                        correlationId);
                    await _eventBus.Publish(debitedEvent);

                    // Step 3: Event 3 - Settled Event
                    var settledEvent = new FundTransferSettledIntegrationEvent(
                        transactionRecord.TransactionId,
                        request.accountId,
                        request.amount,
                        $"INTERNAL_SETTLED_{transactionRecord.TransactionId:N}",
                        DateTime.UtcNow,
                        correlationId);
                    await _eventBus.Publish(settledEvent);

                    // Save to MongoDB for read/audit log
                    if (_mongoService != null)
                    {
                        await _mongoService.SaveAccountDetailAsync(withdrawAccount);
                        await _mongoService.SaveAccountDetailAsync(depositAccount);
                        await _mongoService.SaveTransferTransactionAsync(transactionRecord);
                    }

                    logger.LogInformation("Internal transfer completed successfully for TransactionId: {TransactionId}", transactionRecord.TransactionId);
                    return true;
                }
            }

            // Scenario 2: Inter-Bank Transfer using Payment Gateway (NEFT / RTGS / UPI to another bank)
            // 1. Debit origin account in local DB
            withdrawAccount.Withdraw(request.accountId, request.amount, $"Transfer via {request.transferType} / {request.paymentGateway}");
            _repository.Update(withdrawAccount);

            // 2. Create pending transfer record in DB & MongoDB
            var pendingTransaction = FundTransferTransaction.Create(
                request.accountId,
                request.destinationAccountId == Guid.Empty ? null : request.destinationAccountId,
                request.amount,
                request.transferType,
                request.paymentGateway,
                request.beneficiaryAccountNo,
                request.ifscCode,
                request.upiId,
                request.destinationBankName,
                request.description ?? $"Pending Inter-Bank Transfer via {request.transferType}",
                TransferStatus.Pending);

            _transferRepository.Insert(pendingTransaction);
            await this.UnitOfWork.CommitAsync();

            if (_mongoService != null)
            {
                await _mongoService.SaveAccountDetailAsync(withdrawAccount);
                await _mongoService.SaveTransferTransactionAsync(pendingTransaction);
            }

            // Step 2: Event 2 - Publish AccountDebitedIntegrationEvent (Triggers SMS/Notification Engine)
            var interBankDebitedEvent = new AccountDebitedIntegrationEvent(
                pendingTransaction.TransactionId,
                request.accountId,
                request.amount,
                DateTime.UtcNow,
                $"Debited for inter-bank transfer via {request.transferType}",
                correlationId);
            await _eventBus.Publish(interBankDebitedEvent);

            // Step 3: Event 3 - Publish SentToClearingIntegrationEvent (Triggers Clearing Audit / Analytics)
            var clearingEvent = new SentToClearingIntegrationEvent(
                pendingTransaction.TransactionId,
                request.accountId,
                request.amount,
                request.transferType,
                request.paymentGateway,
                request.beneficiaryAccountNo,
                request.ifscCode,
                request.upiId,
                DateTime.UtcNow,
                correlationId);
            await _eventBus.Publish(clearingEvent);

            // Step 4: Publish FundTransferRequestedIntegrationEvent to persistent queue (Processed asynchronously by Payment Processor)
            var transferEvent = new FundTransferRequestedIntegrationEvent(
                pendingTransaction.TransactionId,
                request.accountId,
                request.destinationAccountId,
                request.amount,
                request.transferType,
                request.paymentGateway,
                request.beneficiaryAccountNo,
                request.ifscCode,
                request.upiId,
                request.destinationBankName,
                request.description ?? "Fund transfer request",
                correlationId);

            await _eventBus.Publish(transferEvent);
            logger.LogInformation("Published all lifecycle events and queued FundTransferRequestedIntegrationEvent for TransactionId: {TransactionId}", pendingTransaction.TransactionId);

            return true;
        }
    }
}
