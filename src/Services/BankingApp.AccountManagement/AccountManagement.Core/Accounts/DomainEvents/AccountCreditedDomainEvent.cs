using BankingAppDDD.Domains.Abstractions.DomainEvents;
using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingAppDDD.Domains.Accounts.DomainEvents
{
    public record class AccountCreditedDomainEvent : DomainEvent
    {
        public Guid AccountId { get; private set; }
        public Amount Amount { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public string Description { get; private set; }
        private AccountCreditedDomainEvent(Guid accountId, Amount amount, DateTime transactionDate, string description)
        {
            this.AccountId = accountId;
            this.Amount = amount;
            this.TransactionDate = transactionDate;
            this.Description = description;
        }
        public static AccountCreditedDomainEvent Create(Guid accountId, Amount amount, DateTime transactionDate, string description)
        {
            if (accountId == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(accountId));
            if (amount.Value == 0m)
                throw new ArgumentOutOfRangeException(nameof(amount));
            return new AccountCreditedDomainEvent(accountId, amount, transactionDate, description);
        }
    }
}
