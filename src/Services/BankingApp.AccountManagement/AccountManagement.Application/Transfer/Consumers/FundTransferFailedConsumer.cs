using BankingAppDDD.Applications.Abstractions.IntegrationEvents.Transfer;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.MongoService.Application.Mongo;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Transfer.Consumers
{
    public sealed class FundTransferFailedConsumer : IConsumer<FundTransferFailedIntegrationEvent>
    {
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<FundTransferTransaction> _transferRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountMongoService? _mongoService;
        private readonly ILogger<FundTransferFailedConsumer> _logger;

        public FundTransferFailedConsumer(
            IRepository<Account> accountRepository,
            IRepository<FundTransferTransaction> transferRepository,
            IUnitOfWork unitOfWork,
            ILogger<FundTransferFailedConsumer> logger,
            IAccountMongoService? mongoService = null)
        {
            _accountRepository = accountRepository;
            _transferRepository = transferRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mongoService = mongoService;
        }

        public async Task Consume(ConsumeContext<FundTransferFailedIntegrationEvent> context)
        {
            var @event = context.Message;
            _logger.LogWarning("Consumed FundTransferFailedIntegrationEvent for TransactionId: {TransactionId}, Reason: {Reason}. Initiating Transaction Reversal...",
                @event.TransactionId, @event.FailureReason);

            var transaction = await _transferRepository.GetByIdAsync(@event.TransactionId);
            if (transaction != null)
            {
                // Mark transaction as failed due to gateway/network retry exhaustion
                transaction.MarkFailed(@event.FailureReason);
                _transferRepository.Update(transaction);
                await _unitOfWork.CommitAsync();

                if (_mongoService != null)
                {
                    await _mongoService.SaveTransferTransactionAsync(transaction);
                }

                // Trigger Transaction Reversal Event
                var reversalEvent = new FundTransferReversalRequestedIntegrationEvent(
                    @event.TransactionId,
                    @event.AccountId,
                    @event.Amount,
                    $"Gateway/Clearing Network transfer failed after retries: {@event.FailureReason}",
                    @event.CorrelationId);

                await context.Publish(reversalEvent);
                _logger.LogInformation("Published FundTransferReversalRequestedIntegrationEvent for TransactionId: {TransactionId}", @event.TransactionId);
            }
            else
            {
                _logger.LogWarning("FundTransferTransaction {TransactionId} not found for failure processing", @event.TransactionId);
            }
        }
    }
}
