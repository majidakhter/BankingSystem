using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingApp.AccountManagement.Core.Accounts.Entities
{
    public sealed class Credit : EntityBase
    {
        private Credit()
        {

        }
        private Credit(Guid accountId, Amount amount, DateTime createdAt, string description)
        {
            this.AccountId = accountId;
            this.Amount = amount;
            this.CreatedAt = createdAt;
            this.Description = description;
        }
        public Guid AccountId { get; private set; }
        public Amount Amount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string Description { get; private set; }
        public static Credit Create(Guid accountId, Amount amount, DateTime createdAt, string description)
        {
            return new Credit(accountId, amount, createdAt, description);
        }

    }
}
