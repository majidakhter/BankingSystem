
namespace BankingAppDDD.AccountManagement.Application.Outbox
{
    public interface IOutboxService
    {
        Task SaveEventAsync<TEvent>(string aggregateType, string aggregateId, TEvent integrationEvent) where TEvent : class;
    }
}
