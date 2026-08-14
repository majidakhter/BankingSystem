using BankingAppDDD.Applications.Abstractions.IntegrationEvents.Transfer;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.Domains.Accounts.Models;
using BankingAppDDD.MongoService.Application.Mongo;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Transfer.Consumers
{
    public sealed class FundTransferReversalConsumer : IConsumer<FundTransferReversalRequestedIntegrationEvent>
    {
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<FundTransferTransaction> _transferRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountMongoService? _mongoService;
        private readonly ILogger<FundTransferReversalConsumer> _logger;

        public FundTransferReversalConsumer(
            IRepository<Account> accountRepository,
            IRepository<FundTransferTransaction> transferRepository,
            IUnitOfWork unitOfWork,
            ILogger<FundTransferReversalConsumer> logger,
            IAccountMongoService? mongoService = null)
        {
            _accountRepository = accountRepository;
            _transferRepository = transferRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mongoService = mongoService;
        }

        public async Task Consume(ConsumeContext<FundTransferReversalRequestedIntegrationEvent> context)
        {
            var @event = context.Message;
            _logger.LogWarning("Processing Transaction Reversal for TransactionId: {TransactionId}, AccountId: {AccountId}, Amount: {Amount}, Reason: {Reason}",
                @event.TransactionId, @event.AccountId, @event.Amount, @event.ReversalReason);

            var transaction = await _transferRepository.GetByIdAsync(@event.TransactionId);
            if (transaction != null)
            {
                if (transaction.Status == TransferStatus.Reversed)
                {
                    _logger.LogInformation("Transaction {TransactionId} has already been reversed.", @event.TransactionId);
                    return;
                }

                // 1. Update status to Reversed
                transaction.MarkReversed(@event.ReversalReason);
                _transferRepository.Update(transaction);

                // 2. Credit (Refund) original debited amount back to sender's bank account
                var account = await _accountRepository.GetByIdAsync(@event.AccountId);
                if (account is Account sourceAccount)
                {
                    sourceAccount.Deposit(@event.AccountId, @event.Amount, $"Reversal for failed transaction: {@event.ReversalReason}");
                    _accountRepository.Update(sourceAccount);

                    if (_mongoService != null)
                    {
                        await _mongoService.SaveAccountDetailAsync(sourceAccount);
                    }
                }

                // 3. Commit PostgreSQL transaction
                await _unitOfWork.CommitAsync();

                // 4. Save reversal entry in MongoDB audit trail
                if (_mongoService != null)
                {
                    await _mongoService.SaveTransferTransactionAsync(transaction);
                }

                // 5. Event 5 - Publish FundTransferReversedIntegrationEvent (Listened by Notification Engine & Risk)
                var reversedEvent = new FundTransferReversedIntegrationEvent(
                    transaction.TransactionId,
                    transaction.AccountId,
                    transaction.Amount,
                    transaction.CurrencyCode,
                    @event.ReversalReason,
                    DateTime.UtcNow,
                    @event.CorrelationId);

                await context.Publish(reversedEvent);
                _logger.LogInformation("Transaction Reversal executed successfully for TransactionId: {TransactionId} and published FundTransferReversedIntegrationEvent", @event.TransactionId);
            }
            else
            {
                _logger.LogError("Transaction Reversal failed: FundTransferTransaction {TransactionId} not found.", @event.TransactionId);
            }
        }
    }
}
