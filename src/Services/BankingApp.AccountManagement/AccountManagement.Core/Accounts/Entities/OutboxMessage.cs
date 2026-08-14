using System;

namespace BankingAppDDD.Domains.Accounts.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string AggregateType { get; set; } = string.Empty;
        public string AggregateId { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
    }
}
