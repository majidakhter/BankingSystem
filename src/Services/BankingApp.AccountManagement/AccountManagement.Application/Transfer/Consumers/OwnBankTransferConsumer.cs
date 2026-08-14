using BankingAppDDD.Applications.Abstractions.IntegrationEvents.Transfer;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.Domains.Accounts.Models;
using BankingAppDDD.MongoService.Application.Mongo;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Transfer.Consumers
{
    public sealed class OwnBankTransferConsumer : IConsumer<FundTransferRequestedIntegrationEvent>
    {
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<BeneficiaryGroup> _beneficiaryRepository;
        private readonly IRepository<FundTransferTransaction> _transferRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountMongoService? _mongoService;
        private readonly ILogger<OwnBankTransferConsumer> _logger;

        public OwnBankTransferConsumer(
            IRepository<Account> accountRepository,
            IRepository<BeneficiaryGroup> beneficiaryRepository,
            IRepository<FundTransferTransaction> transferRepository,
            IUnitOfWork unitOfWork,
            ILogger<OwnBankTransferConsumer> logger,
            IAccountMongoService? mongoService = null)
        {
            _accountRepository = accountRepository;
            _beneficiaryRepository = beneficiaryRepository;
            _transferRepository = transferRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mongoService = mongoService;
        }

        public async Task Consume(ConsumeContext<FundTransferRequestedIntegrationEvent> context)
        {
            var @event = context.Message;

            // Only process OwnBankAccount transfers in this consumer
            if (@event.transferToEntity != TransferToEntity.OwnBankAccount)
            {
                return;
            }

            _logger.LogInformation("Processing OwnBankTransferConsumer for TransactionId: {TransactionId}, Amount: {Amount}",
                @event.TransactionId, @event.Amount);

            try
            {
                Account? destinationAccount = null;

                // 1. Try finding destination account by Guid AccountId
                if (@event.DestinationAccountId.HasValue && @event.DestinationAccountId.Value != Guid.Empty)
                {
                    destinationAccount = await _accountRepository.GetByIdAsync(@event.DestinationAccountId.Value);
                }

                // 2. Try finding destination account by string/int AccountNo
                if (destinationAccount == null && !string.IsNullOrWhiteSpace(@event.BeneficiaryAccountNo))
                {
                    if (int.TryParse(@event.BeneficiaryAccountNo, out int acctNoInt))
                    {
                        destinationAccount = await _accountRepository.FirstOrDefaultAsync(a => a.AccountNo == acctNoInt);
                    }

                    if (destinationAccount == null)
                    {
                        destinationAccount = await _accountRepository.FirstOrDefaultAsync(a => a.AccountNo.ToString() == @event.BeneficiaryAccountNo);
                    }

                    // 3. Fallback: try finding via beneficiary record for IMPS / mobile number lookup
                    if (destinationAccount == null)
                    {
                        var beneficiary = await _beneficiaryRepository.FirstOrDefaultAsync(b => b.Beneficiary != null && b.Beneficiary.BeneficaryAccountNo.ToString() == @event.BeneficiaryAccountNo);
                        if (beneficiary != null)
                        {
                            destinationAccount = await _accountRepository.FirstOrDefaultAsync(a => a.AccountNo == beneficiary.Beneficiary.BeneficaryAccountNo);
                        }
                    }
                }

                if (destinationAccount != null)
                {
                    // Credit destination account
                    Guid senderAccountId = @event.AccountId;
                    destinationAccount.Deposit(senderAccountId, @event.Amount, @event.Description ?? "Deposited via internal transfer");
                    _accountRepository.Update(destinationAccount);

                    // Update transaction record to Completed
                    var transaction = await _transferRepository.GetByIdAsync(@event.TransactionId);
                    if (transaction != null)
                    {
                        string gatewayRef = $"INTERNAL_SETTLED_{transaction.TransactionId:N}";
                        transaction.MarkCompleted(gatewayRef);
                        _transferRepository.Update(transaction);
                    }

                    await _unitOfWork.CommitAsync();

                    // Save state snapshots to MongoDB
                    if (_mongoService != null)
                    {
                        await _mongoService.SaveAccountDetailAsync(destinationAccount);
                        if (transaction != null)
                        {
                            await _mongoService.SaveTransferTransactionAsync(transaction);
                        }
                    }

                    // Publish completion and settlement events
                    var completedEvent = new FundTransferCompletedIntegrationEvent(
                        @event.TransactionId,
                        @event.AccountId,
                        $"INTERNAL_SETTLED_{@event.TransactionId:N}",
                        @event.CorrelationId);

                    await context.Publish(completedEvent);

                    _logger.LogInformation("Successfully completed internal transfer for TransactionId: {TransactionId}", @event.TransactionId);
                }
                else
                {
                    _logger.LogWarning("Destination account not found for internal transfer TransactionId: {TransactionId}. Initiating failure...", @event.TransactionId);
                    var failedEvent = new FundTransferFailedIntegrationEvent(
                        @event.TransactionId,
                        @event.AccountId,
                        @event.Amount,
                        @event.currencyCode,
                        "Destination account not found for internal transfer",
                        @event.CorrelationId);

                    await context.Publish(failedEvent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in OwnBankTransferConsumer for TransactionId: {TransactionId}", @event.TransactionId);
                var failedEvent = new FundTransferFailedIntegrationEvent(
                    @event.TransactionId,
                    @event.AccountId,
                    @event.Amount,
                    @event.currencyCode,
                    ex.Message,
                    @event.CorrelationId);

                await context.Publish(failedEvent);
            }
        }
    }
}
