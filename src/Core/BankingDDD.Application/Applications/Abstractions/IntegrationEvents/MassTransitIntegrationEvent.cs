using BankingAppDDD.Domains.Abstractions.Models;

namespace BankingAppDDD.Applications.Abstractions.IntegrationEvents
{
    public abstract record MassTransitIntegrationEvent(string CorrelationId);
    public abstract record MassTransitIntegrationEventWithStatus(Guid CorrelationId, LoanApplicationStatus status);
}
