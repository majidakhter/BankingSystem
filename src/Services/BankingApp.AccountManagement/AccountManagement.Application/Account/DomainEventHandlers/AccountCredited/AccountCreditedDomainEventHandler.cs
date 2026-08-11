using BankingAppDDD.Applications.Abstractions.DomainEventHandlers;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Accounts.DomainEvents;
using BankingAppDDD.Domains.CustomerAccounts.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Accounts.DomainEventHandlers.AccountCredited
{
    /*public sealed class AccountCreditedDomainEventHandler : DomainEventHandler<AccountCreditedDomainEvent>
    {
        private readonly IBus _eventBus;
        private readonly IRepository<UserAccount> _customerRepository;
        public AccountCreditedDomainEventHandler(IRepository<UserAccount> customerRepository, ILogger<DomainEventHandler<AccountCreditedDomainEvent>> logger,
            IBus eventBus) : base(logger)
        {
            _eventBus = eventBus;
            _customerRepository = customerRepository;
        }

        protected override async Task OnHandleAsync(AccountCreditedDomainEvent @event)
        {
            await _eventBus.Publish(AccountCreditedDomainEvent.Create(@event.AccountId, @event.Amount, @event.TransactionDate, @event.Description));
            var customer = await _customerRepository.FirstOrDefaultAsync(q => q.UserId == Guid.Empty);//TODO
            customer.SetOneAccountAdded();
            _customerRepository.Update(customer);

        }
    }*/
}
