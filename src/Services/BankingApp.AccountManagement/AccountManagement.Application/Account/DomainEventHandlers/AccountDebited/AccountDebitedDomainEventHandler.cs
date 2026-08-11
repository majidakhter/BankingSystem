using BankingAppDDD.Applications.Abstractions.DomainEventHandlers;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Accounts.DomainEvents;
using BankingAppDDD.Domains.CustomerAccounts.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Accounts.DomainEventHandlers.AccountDebited
{
    public sealed class AccountDebitedDomainEventHandler : DomainEventHandler<AccountDebitedDomainEvent>
    {
        private readonly IBus _eventBus;
        private readonly IRepository<UserAccount> _customerRepository;
        public AccountDebitedDomainEventHandler(IRepository<UserAccount> customerRepository, ILogger<DomainEventHandler<AccountDebitedDomainEvent>> logger,
            IBus eventBus) : base(logger)
        {
            _eventBus = eventBus;
            _customerRepository = customerRepository;
        }

        protected override async Task OnHandleAsync(AccountDebitedDomainEvent @event)
        {
            await _eventBus.Publish(AccountDebitedDomainEvent.Create(@event.AccountId, @event.Amount, @event.TransactionDate, @event.Description));
            //var customer = await _customerRepository.FirstOrDefaultAsync(q => q.CustomerId == @event.U);
            //customer.SetOneAccountAdded();
            //_customerRepository.Update(customer);

        }
    }
}
