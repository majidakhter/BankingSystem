using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace BankingApp.AccountManagement.Infrastructure.Outbox
{
    public class OutboxProcessorBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxProcessorBackgroundService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromSeconds(3);
        private readonly AsyncRetryPolicy _publishRetryPolicy;

        public OutboxProcessorBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<OutboxProcessorBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            _publishRetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning("Outbox Publisher Retry {RetryCount}: Transient error encountered. Retrying in {SleepSeconds}s. Exception: {Message}",
                            retryCount, timeSpan.TotalSeconds, exception.Message);
                    });
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
                        eventType = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(a => a.GetTypes())
                            .FirstOrDefault(t => t.FullName == message.EventType || t.Name == message.EventType);
                    }

                    if (eventType != null)
                    {
                        var eventObj = JsonSerializer.Deserialize(message.Payload, eventType);
                        if (eventObj != null)
                        {
                            await _publishRetryPolicy.ExecuteAsync(async () =>
                            {
                                await publishEndpoint.Publish(eventObj, eventType, cancellationToken);
                            });

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
