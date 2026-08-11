using BankingAppDDD.Domains.Abstractions.DomainEvents;

namespace BankingAppDDD.Domains.Accounts.DomainEvents
{
    public record class AccountAddedDomainEvent : DomainEvent
    {
        public Guid AccountId { get; private set; }
        public Guid UserId { get; private set; }
        public Guid KeycloakUserId { get; private set; }
        public int AccountTypeId { get; private set; }
        private AccountAddedDomainEvent(Guid accountId, Guid userId, Guid keycloakUserId, int accountTypeId)
        {
            this.AccountId = accountId;
            this.UserId = userId;
            this.KeycloakUserId = keycloakUserId;
            this.AccountTypeId = accountTypeId;
        }
        public static AccountAddedDomainEvent Create(Guid accountId, Guid userId, Guid keycloakUserId, int accountTypeId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(userId));
            if (keycloakUserId == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(keycloakUserId));
            if (accountId == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(accountId));
            return new AccountAddedDomainEvent(accountId, userId, keycloakUserId, accountTypeId);
        }
    }
}
