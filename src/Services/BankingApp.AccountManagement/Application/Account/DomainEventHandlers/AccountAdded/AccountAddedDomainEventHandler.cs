using BankingApp.AccountManagement.Core.Accounts.DomainEvents;
using BankingApp.AccountManagement.Core.Customers.Entities;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.DomainEventHandlers;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.AccountManagement.Application.Accounts.DomainEventHandlers.AccountAdded
{
    public sealed class AccountAddedDomainEventHandler : DomainEventHandler<AccountAddedDomainEvent>
    {

        private readonly IBus _eventBus;
        private readonly IAccountRepository<Customer> _customerRepository;
        public AccountAddedDomainEventHandler(IAccountRepository<Customer> customerRepository, ILogger<DomainEventHandler<AccountAddedDomainEvent>> logger,
            IBus eventBus) : base(logger)
        {
            _eventBus = eventBus;
            _customerRepository = customerRepository;
        }

        protected override async Task OnHandleAsync(AccountAddedDomainEvent @event)
        {
            await _eventBus.Publish(AccountAddedDomainEvent.Create(@event.AccountId, @event.CustomerId, @event.AccountTypeId));
            var customer = await _customerRepository.FirstOrDefaultAsync(q => q.CustomerId == @event.CustomerId);
            customer.SetOneAccountAdded();
            _customerRepository.Update(customer);

        }
       
    }
}
