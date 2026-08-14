using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Infrastructure.Outbox
{
    /// <summary>
    /// background worker reads the outbox table, publishes the event to the message broker safely, and marks it as processed.
    /// Initiate Transfer: User transfers $100 from Account A (Service 1) to Account B (Service 2).
    /// Local Write: Service 1 runs a local transaction:Subtracts $100 from Account A’s balance.
    /// Inserts row: Event: MoneyWithdrawn, Amount: $100, Status: Pending into the local outbox table
    /// Commit: The database commits successfully. No messages are lost even if the app crashes next.
    /// Relay Event: A background poller reads the pending outbox row and pushes it to the message broker.
    /// Acknowledge: The poller updates the outbox row to Status: Processed.
    /// Receiver Update: Service 2 consumes the event and safely adds $100 to Account B
    /// </summary>
    public class OutboxProcessorBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxProcessorBackgroundService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromSeconds(3);

        public OutboxProcessorBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<OutboxProcessorBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OutboxProcessorBackgroundService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessOutboxMessagesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing outbox messages.");
                }

                await Task.Delay(_period, stoppingToken);
            }

            _logger.LogInformation("OutboxProcessorBackgroundService stopped.");
        }

        private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            var messages = await dbContext.OutboxMessages
                .Where(m => m.ProcessedAt == null)
                .OrderBy(m => m.CreatedAt)
                .Take(20)
                .ToListAsync(cancellationToken);

            if (!messages.Any())
            {
                return;
            }

            _logger.LogInformation("Processing {Count} outbox messages...", messages.Count);

            foreach (var message in messages)
            {
                try
                {
                    var eventType = Type.GetType(message.EventType);
                    if (eventType == null)
                    {
                        // Fallback type resolution by class name across assemblies
                        eventType = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(a => a.GetTypes())
                            .FirstOrDefault(t => t.FullName == message.EventType || t.Name == message.EventType);
                    }

                    if (eventType != null)
                    {
                        var eventObj = JsonSerializer.Deserialize(message.Payload, eventType);
                        if (eventObj != null)
                        {
                            await publishEndpoint.Publish(eventObj, eventType, cancellationToken);
                            _logger.LogInformation("Successfully published outbox event {EventType} for AggregateId {AggregateId}", message.EventType, message.AggregateId);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Could not resolve Type for event_type {EventType}", message.EventType);
                    }

                    message.ProcessedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish outbox event {Id} of type {EventType}", message.Id, message.EventType);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
