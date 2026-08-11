using BankingAppDDD.Applications.Abstractions.DomainEventHandlers;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Accounts.DomainEvents;
using BankingAppDDD.Domains.CustomerAccounts.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Accounts.DomainEventHandlers.AccountClosed
{
    public sealed class AccountClosedDomainEventHandler : DomainEventHandler<AccountClosedDomainEvent>
    {
        private readonly IBus _eventBus;
        private readonly IRepository<UserAccount> _customerRepository;
        public AccountClosedDomainEventHandler(IRepository<UserAccount> customerRepository, ILogger<DomainEventHandler<AccountClosedDomainEvent>> logger,
            IBus eventBus) : base(logger)
        {
            _eventBus = eventBus;
            _customerRepository = customerRepository;
        }

        protected override async Task OnHandleAsync(AccountClosedDomainEvent @event)
        {
            await _eventBus.Publish(AccountClosedDomainEvent.Create(@event.AccountId, @event.UserId));
            var customer = await _customerRepository.FirstOrDefaultAsync(q => q.UserId == @event.UserId);
            customer.SetOneAccountClosed();
            _customerRepository.Update(customer);

        }
    }
}
