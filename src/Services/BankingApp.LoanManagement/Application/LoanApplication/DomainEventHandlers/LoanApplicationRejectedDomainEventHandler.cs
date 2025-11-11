using BankingApp.LoanManagement.Core.LoanApplicationsDomainEvents;
using BankingAppDDD.Applications.Abstractions.DomainEventHandlers;
using BankingAppDDD.Applications.Abstractions.IntegrationEvents.LoanEvents;
using MassTransit;

namespace BankingApp.LoanManagement.Application.LoanApplicationDomainEventHandlers
{
    public sealed class LoanApplicationRejectedDomainEventHandler : DomainEventHandler<LoanApplicationRejected>
    {
        private readonly IBus _eventBus;

        public LoanApplicationRejectedDomainEventHandler(ILogger<DomainEventHandler<LoanApplicationRejected>> logger,
            IBus eventBus) : base(logger)
        {
            _eventBus = eventBus;
        }

        protected override async Task OnHandleAsync(LoanApplicationRejected @event)
        {
            var loanApplicationCreatedEvent = new LoanApplicationCreatedEvent(
                 @event.CustomerId,
                 @event.LoanApplicationStatus
             );
            await _eventBus.Publish(loanApplicationCreatedEvent);
        }
    }
}
