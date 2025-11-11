
using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingApp.AccountManagement.Core.Accounts.Entities
{
    public sealed class Debit : EntityBase
    {
        private Debit()
        {

        }
        private Debit(Guid accountId, Amount amount, DateTime createdAt, string description)
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
        public  Account Account { get; private set; }
        public  static Debit Create(Guid accountId, Amount amount, DateTime createdAt, string description)
        {
            return new Debit(accountId, amount, createdAt,description);
        }
    }
}
