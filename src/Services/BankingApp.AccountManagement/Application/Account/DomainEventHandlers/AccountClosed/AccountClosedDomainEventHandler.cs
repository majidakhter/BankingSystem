using BankingApp.AccountManagement.Core.Accounts.DomainEvents;
using BankingApp.AccountManagement.Core.Customers.Entities;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.DomainEventHandlers;
using MassTransit;

namespace BankingApp.AccountManagement.Application.Accounts.DomainEventHandlers.AccountClosed
{
    public sealed class AccountClosedDomainEventHandler : DomainEventHandler<AccountClosedDomainEvent>
    {
        private readonly IBus _eventBus;
        private readonly IAccountRepository<Customer> _customerRepository;
        public AccountClosedDomainEventHandler(IAccountRepository<Customer> customerRepository, ILogger<DomainEventHandler<AccountClosedDomainEvent>> logger,
            IBus eventBus) : base(logger)
        {
            _eventBus = eventBus;
            _customerRepository = customerRepository;
        }

        protected override async Task OnHandleAsync(AccountClosedDomainEvent @event)
        {
            await _eventBus.Publish(AccountClosedDomainEvent.Create(@event.AccountId, @event.CustomerId));
            var customer = await _customerRepository.FirstOrDefaultAsync(q => q.Id == @event.CustomerId);
            customer.SetOneAccountClosed();
            _customerRepository.Update(customer);

        }
    }
}
