using BankingAppDDD.Applications.Abstractions.DomainEventHandlers;
using BankingAppDDD.Applications.Abstractions.IntegrationEvents.CustomerEvents;
using BankingAppDDD.CustomerManagement.Core.Customers.DomainEvents;
using MassTransit;

namespace BankingAppDDD.CustomerManagement.Application.Customers.DomainEventHandlers
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CustomerRegisteredDomainEventHandler : DomainEventHandler<CustomerRegistered>
    {
        private readonly IBus _eventBus;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventBus"></param>
        public CustomerRegisteredDomainEventHandler(ILogger<DomainEventHandler<CustomerRegistered>> logger,
            IBus eventBus) : base(logger)
        {
            _eventBus = eventBus;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        protected override async Task OnHandleAsync(CustomerRegistered @event)
        {
            if (@event.CustType != 3)
            {
                var customerCreatedEvent = new CustomerCreatedEvent(
                    @event.CustomerId
                );
                await _eventBus.Publish(customerCreatedEvent);
            }
        }
    }
}
