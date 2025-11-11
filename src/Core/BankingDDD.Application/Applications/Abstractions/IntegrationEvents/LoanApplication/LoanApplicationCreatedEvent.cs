

using BankingAppDDD.Domains.Abstractions.Models;

namespace BankingAppDDD.Applications.Abstractions.IntegrationEvents.LoanEvents
{
    public sealed record LoanApplicationCreatedEvent(Guid id, LoanApplicationStatus status) : MassTransitIntegrationEventWithStatus(id, status);
}
