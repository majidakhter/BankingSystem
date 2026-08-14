using System;
using System.Text.Json;
using System.Threading.Tasks;
using BankingApp.AccountManagement;
using BankingAppDDD.AccountManagement.Infrastructure.Outbox;
using BankingAppDDD.Domains.Accounts.Entities;

namespace BankingApp.AccountManagement.Infrastructure.Outbox
{
    public class OutboxService : IOutboxService
    {
        private readonly AccountDbContext _dbContext;

        public OutboxService(AccountDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveEventAsync<TEvent>(string aggregateType, string aggregateId, TEvent integrationEvent) where TEvent : class
        {
            if (integrationEvent == null) return;

            var payload = JsonSerializer.Serialize(integrationEvent, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                AggregateType = aggregateType,
                AggregateId = aggregateId,
                EventType = typeof(TEvent).AssemblyQualifiedName ?? typeof(TEvent).FullName ?? typeof(TEvent).Name,
                Payload = payload,
                CreatedAt = DateTime.UtcNow,
                ProcessedAt = null
            };

            await _dbContext.OutboxMessages.AddAsync(outboxMessage);
        }
    }
}
