using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.AccountManagement.Application.Outbox;
using BankingAppDDD.Applications.Abstractions.IntegrationEvents.Transfer;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.Domains.Accounts.Models;
using BankingAppDDD.MongoService.Application.Mongo;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BankingApp.AccountManagement.Application.Transfer.Consumers
{
    /// <summary>
    /// Two-Phase / State Machine Consumer handling FraudEvaluationCompletedIntegrationEvent.
    /// Updates transaction state machine on the ledger/database based on async risk evaluation callback:
    /// - ALLOW -> Debits account balance, transitions state PendingVerification -> Processing -> Completed, publishes SentToClearingIntegrationEvent.
    /// - TRIGGER_MFA_STEP_UP -> Transitions state to RequiresOtp.
    /// - BLOCK_TRANSACTION -> Transitions state to Rejected, publishes FundTransferRejectedIntegrationEvent without debiting.
    /// </summary>
    public sealed class TransferStateProcessorConsumer : IConsumer<FraudEvaluationCompletedIntegrationEvent>
    {
        private readonly IAccountRepository<Account> _accountRepository;
        private readonly IRepository<FundTransferTransaction> _transferRepository;
        private readonly IOutboxService _outboxService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountMongoService? _mongoService;
        private readonly ILogger<TransferStateProcessorConsumer> _logger;

        public TransferStateProcessorConsumer(
            IAccountRepository<Account> accountRepository,
            IRepository<FundTransferTransaction> transferRepository,
            IOutboxService outboxService,
            IUnitOfWork unitOfWork,
            ILogger<TransferStateProcessorConsumer> logger,
            IAccountMongoService? mongoService = null)
        {
            _accountRepository = accountRepository;
            _transferRepository = transferRepository;
            _outboxService = outboxService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mongoService = mongoService;
        }

        public async Task Consume(ConsumeContext<FraudEvaluationCompletedIntegrationEvent> context)
        {
            var @event = context.Message;
            _logger.LogInformation("TransferStateProcessorConsumer processing FraudEvaluationCompletedIntegrationEvent for TransactionId: {TransactionId}, Action: {Action}, Score: {Score:F2}",
                @event.TransactionId, @event.Action, @event.EnsembleScore);

            var transaction = await _transferRepository.GetByIdAsync(@event.TransactionId);
            if (transaction == null)
            {
                _logger.LogWarning("Transaction {TransactionId} not found for state transition processing", @event.TransactionId);
                return;
            }

            // Two-Phase State Machine Lifecycle Transitions
            switch (@event.Action)
            {
                case "BLOCK_TRANSACTION":
                    // State Transition: PendingVerification -> Rejected
                    transaction.MarkFailed($"Rejected by Asynchronous Ensemble ML & LLM Fraud Engine (Score: {@event.EnsembleScore * 100:F0}%)");
                    _transferRepository.Update(transaction);

                    var rejectedEvent = new FundTransferRejectedIntegrationEvent(
                        transaction.TransactionId,
                        @event.AccountId,
                        @event.Amount,
                        @event.currencyCode,
                        "Blocked by Asynchronous Ensemble ML & LLM Fraud Engine",
                        @event.RiskFactors,
                        DateTime.UtcNow,
                        @event.CorrelationId);

                    await _outboxService.SaveEventAsync("FundTransferTransaction", transaction.TransactionId.ToString(), rejectedEvent);
                    await _unitOfWork.CommitAsync();

                    if (_mongoService != null)
                    {
                        await _mongoService.SaveTransferTransactionAsync(transaction);
                    }

                    _logger.LogWarning("State Machine Transition: Transaction {TransactionId} state updated to REJECTED. FundTransferRejectedIntegrationEvent published.", transaction.TransactionId);
                    break;

                case "TRIGGER_MFA_STEP_UP":
                    // State Transition: PendingVerification -> RequiresOtp
                    transaction.MarkFailed("MFA_STEP_UP_REQUIRED: Elevated risk score requires OTP step-up verification to complete transfer.");
                    _transferRepository.Update(transaction);
                    await _unitOfWork.CommitAsync();

                    if (_mongoService != null)
                    {
                        await _mongoService.SaveTransferTransactionAsync(transaction);
                    }

                    _logger.LogInformation("State Machine Transition: Transaction {TransactionId} state updated to REQUIRES_OTP.", transaction.TransactionId);
                    break;

                case "ALLOW":
                default:
                    // State Transition: PendingVerification -> Processing -> Completed & Debit Account Balance
                    var sourceAccount = await _accountRepository.GetEntityById(@event.AccountId);
                    if (sourceAccount is Account withdrawAccount)
                    {
                        withdrawAccount.Withdraw(@event.AccountId, @event.Amount, @event.Description ?? $"Transfer via {@event.TransferType}");
                        _accountRepository.Update(withdrawAccount);

                        transaction.MarkCompleted($"Approved by Asynchronous Ensemble Risk Assessment via {@event.TransferType}");
                        _transferRepository.Update(transaction);

                        var debitedEvent = new AccountDebitedIntegrationEvent(
                            transaction.TransactionId,
                            @event.AccountId,
                            @event.Amount,
                            @event.currencyCode,
                            DateTime.UtcNow,
                            $"Debited for transfer via {@event.TransferType}",
                            @event.CorrelationId);

                        var clearingEvent = new SentToClearingIntegrationEvent(
                            transaction.TransactionId,
                            @event.AccountId,
                            @event.senderBankIfscCode,
                            @event.Amount,
                            @event.currencyCode,
                            @event.TransferType,
                            @event.transferToEntity,
                            @event.PaymentGateway,
                            @event.BeneficiaryAccountNo,
                            @event.receiverBankIfscCode,
                            @event.Description,
                            DateTime.UtcNow,
                            @event.CorrelationId);

                        var transferRequestedEvent = new FundTransferRequestedIntegrationEvent(
                            transaction.TransactionId,
                            @event.AccountId,
                            @event.DestinationAccountId,
                            @event.Amount,
                            @event.currencyCode,
                            @event.TransferType,
                            @event.transferToEntity,
                            @event.PaymentGateway,
                            @event.BeneficiaryAccountNo,
                            @event.senderBankIfscCode,
                            @event.receiverBankIfscCode,
                            @event.Description ?? "Fund transfer request",
                            @event.CorrelationId);

                        await _outboxService.SaveEventAsync("FundTransferTransaction", transaction.TransactionId.ToString(), debitedEvent);
                        await _outboxService.SaveEventAsync("FundTransferTransaction", transaction.TransactionId.ToString(), clearingEvent);
                        await _outboxService.SaveEventAsync("FundTransferTransaction", transaction.TransactionId.ToString(), transferRequestedEvent);

                        await _unitOfWork.CommitAsync();

                        if (_mongoService != null)
                        {
                            await _mongoService.SaveAccountDetailAsync(withdrawAccount);
                            await _mongoService.SaveTransferTransactionAsync(transaction);
                        }

                        _logger.LogInformation("State Machine Transition: Transaction {TransactionId} state updated to COMPLETED. Account debited & SentToClearingIntegrationEvent published.", transaction.TransactionId);
                    }
                    break;
            }
        }
    }
}
