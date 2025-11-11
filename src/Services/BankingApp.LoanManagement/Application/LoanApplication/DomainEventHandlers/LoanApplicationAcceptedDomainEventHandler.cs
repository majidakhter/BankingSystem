using BankingApp.LoanManagement.Core.LoanApplicationsDomainEvents;
using BankingAppDDD.Applications.Abstractions.DomainEventHandlers;
using BankingAppDDD.Applications.Abstractions.IntegrationEvents.LoanEvents;
using MassTransit;

namespace BankingApp.LoanManagement.Application.LoanApplicationDomainEventHandlers
{
    public sealed class LoanApplicationAcceptedDomainEventHandler : DomainEventHandler<LoanApplicationAccepted>
    {
        private readonly IBus _eventBus;
        public LoanApplicationAcceptedDomainEventHandler(ILogger<DomainEventHandler<LoanApplicationAccepted>> logger,
            IBus eventBus) : base(logger)
        {
            _eventBus = eventBus;
        }

        protected override async Task OnHandleAsync(LoanApplicationAccepted @event)
        {
            var loanApplicationCreatedEvent = new LoanApplicationCreatedEvent(
                    @event.CustomerId,
                    @event.LoanApplicationStatus
                );
            await _eventBus.Publish(loanApplicationCreatedEvent);

        }
    }
}
