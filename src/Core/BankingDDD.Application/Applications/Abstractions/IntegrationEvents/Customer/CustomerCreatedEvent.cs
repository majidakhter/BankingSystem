
namespace BankingAppDDD.Applications.Abstractions.IntegrationEvents.CustomerEvents
{
    public sealed record CustomerCreatedEvent(Guid id) : MassTransitIntegrationEvent(id);

}
