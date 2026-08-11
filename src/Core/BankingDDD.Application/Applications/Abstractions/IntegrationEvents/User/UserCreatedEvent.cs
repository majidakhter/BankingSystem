
namespace BankingAppDDD.Applications.Abstractions.IntegrationEvents.UserEvents
{
    public sealed record UserCreatedEvent(Guid userId, Guid keycloakUserId, decimal openingBalance, int accountTypeId, string CorrelationId) : MassTransitIntegrationEvent(CorrelationId);

}
