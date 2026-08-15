using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.ValueObjects;
using BankingAppDDD.Domains.Abstractions.ValueObjects.Shared;
using BankingAppDDD.Domains.Accounts.DomainEvents;
using System.ComponentModel.DataAnnotations;

namespace BankingAppDDD.Domains.Accounts.Entities
{
    public sealed class Debit : EntityBase, IAccountNonGenericRepo
    {
        // Update the private parameterless constructor to initialize non-nullable properties with default values.
        private Debit()
        {
            Amount = default!;
            Description = string.Empty;
        }
        private Debit(Guid accountId, Amount amount, DateTime transactionDate, string description)
        {
            this.AccountId = accountId;
            this.Amount = amount;
            this.TransactionDate = transactionDate;
            this.Description = description;
            var @event = AccountDebitedDomainEvent.Create(
                AccountId,
                Amount,
                TransactionDate,
                Description);
            AddDomainEvent(@event);
            Apply(@event);
        }

        private void Apply(AccountDebitedDomainEvent @event)
        {
            Description = @event.Description;
            Amount = @event.Amount;
            TransactionDate = @event.Timestamp;
        }

        [Required]
        public Guid AccountId { get; private set; }

        public int TransactionNo { get; private set; }

        [Required]
        public Amount Amount { get; private set; }
        //public Currency Currency { get; private set; }

        [Required]
        public DateTime TransactionDate { get; private set; }

        [Required]
        public string Description { get; private set; }
        public static Debit Create(Guid accountId, Amount amount, DateTime transactionDate, string description)
        {
            return new Debit(accountId, amount, transactionDate, description);
        }
    }
}
