using BankingAppDDD.Applications.Abstractions.IntegrationEvents.Transfer;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.MongoService.Application.Mongo;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Transfer.Consumers
{
    public sealed class FundTransferCompletedConsumer : IConsumer<FundTransferCompletedIntegrationEvent>
    {
        private readonly IRepository<FundTransferTransaction> _transferRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountMongoService? _mongoService;
        private readonly ILogger<FundTransferCompletedConsumer> _logger;

        public FundTransferCompletedConsumer(
            IRepository<FundTransferTransaction> transferRepository,
            IUnitOfWork unitOfWork,
            ILogger<FundTransferCompletedConsumer> logger,
            IAccountMongoService? mongoService = null)
        {
            _transferRepository = transferRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mongoService = mongoService;
        }

        public async Task Consume(ConsumeContext<FundTransferCompletedIntegrationEvent> context)
        {
            var @event = context.Message;
            _logger.LogInformation("Consumed FundTransferCompletedIntegrationEvent for TransactionId: {TransactionId}, GatewayRef: {GatewayRef}",
                @event.TransactionId, @event.GatewayTransactionRef);

            var transaction = await _transferRepository.GetByIdAsync(@event.TransactionId);
            if (transaction != null)
            {
                transaction.MarkCompleted(@event.GatewayTransactionRef);
                _transferRepository.Update(transaction);
                await _unitOfWork.CommitAsync();

                if (_mongoService != null)
                {
                    await _mongoService.SaveTransferTransactionAsync(transaction);
                }

                // Step 4: Event 4 - Publish Settled Event (Listened by Analytics, Notification Engine, Accounting)
                var settledEvent = new FundTransferSettledIntegrationEvent(
                    transaction.TransactionId,
                    transaction.AccountId,
                    transaction.Amount,
                    @event.GatewayTransactionRef ?? $"SETTLED_{transaction.TransactionId:N}",
                    DateTime.UtcNow,
                    @event.CorrelationId);

                await context.Publish(settledEvent);
                _logger.LogInformation("Successfully marked FundTransferTransaction {TransactionId} as Completed and published FundTransferSettledIntegrationEvent", @event.TransactionId);
            }
            else
            {
                _logger.LogWarning("FundTransferTransaction {TransactionId} not found in database", @event.TransactionId);
            }
        }
    }
}
