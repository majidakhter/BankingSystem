using BankingAppDDD.Applications.Abstractions.DomainEventHandlers;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Accounts.DomainEvents;
using BankingAppDDD.Domains.CustomerAccounts.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Accounts.DomainEventHandlers.AccountAdded
{
    /*public sealed class AccountAddedDomainEventHandler : DomainEventHandler<AccountAddedDomainEvent>
    {
        //private readonly IBus _eventBus;
        private readonly IRepository<UserAccount> _customerRepository;
        public AccountAddedDomainEventHandler(IRepository<UserAccount> customerRepository, ILogger<DomainEventHandler<AccountAddedDomainEvent>> logger,
            ) : base(logger)
        {
            //_eventBus = eventBus;
            _customerRepository = customerRepository;
        }

        protected override async Task OnHandleAsync(AccountAddedDomainEvent @event)
        {
            //await _eventBus.Publish(AccountAddedDomainEvent.Create(@event.AccountId, @event.UserId, @event.KeycloakUserId, @event.AccountTypeId));
            var customer = await _customerRepository.FirstOrDefaultAsync(q => q.UserId == @event.UserId);
            customer.SetOneAccountAdded();
            _customerRepository.Update(customer);
            
        }

    }*/
}
