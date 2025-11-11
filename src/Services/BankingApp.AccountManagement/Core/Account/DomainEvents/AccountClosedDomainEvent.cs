using BankingAppDDD.Domains.Abstractions.DomainEvents;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.AccountManagement.Core.Accounts.DomainEvents
{
    public record class AccountClosedDomainEvent : DomainEvent
    {

        public Guid AccountId { get; private set; }
        public Guid CustomerId { get; private set; }
        private AccountClosedDomainEvent(Guid accountId, Guid customerId)
        {
            this.AccountId = accountId;
            this.CustomerId = customerId;
        }
        public static AccountClosedDomainEvent Create(Guid accountId, Guid customerId)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(customerId));
            if (accountId == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(accountId));
            return new AccountClosedDomainEvent(accountId, customerId);
        }

    }
}
