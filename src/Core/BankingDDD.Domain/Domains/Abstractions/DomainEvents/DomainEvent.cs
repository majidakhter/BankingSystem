
namespace BankingAppDDD.Domains.Abstractions.DomainEvents
{
    public record class DomainEvent : IDomainEvent
    {
        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }
}
