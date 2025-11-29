using BankingAppDDD.Domains.Abstractions.DomainEvents;

namespace BankingApp.AccountManagement.Core.Accounts.DomainEvents
{
    public record class AccountAddedDomainEvent : DomainEvent
    {
        public Guid AccountId { get; private set; }
        public Guid CustomerId { get; private set; }
        public int AccountTypeId { get; private set; }
        private AccountAddedDomainEvent(Guid accountId, Guid customerId, int accountTypeId)
        {
            this.AccountId = accountId;
            this.CustomerId = customerId;
            this.AccountTypeId = accountTypeId;
        }
        public static AccountAddedDomainEvent Create(Guid accountId, Guid customerId, int accountTypeId)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(customerId));
            if (accountId == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(accountId));
            return new AccountAddedDomainEvent(accountId, customerId, accountTypeId);
        }
    }
}
