using BankingAppDDD.Domains.Abstractions.DomainEvents;


namespace BankingAppDDD.Domains.Accounts.DomainEvents
{
    public record class AccountClosedDomainEvent : DomainEvent
    {
        public Guid AccountId { get; private set; }
        public Guid UserId { get; private set; }
       
        private AccountClosedDomainEvent(Guid accountId, Guid userId)
        {
            this.AccountId = accountId;
            this.UserId = userId;
            
        }
        public static AccountClosedDomainEvent Create(Guid accountId, Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(userId));
            if (accountId == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(accountId));
            return new AccountClosedDomainEvent(accountId, userId);
        }

    }
}
