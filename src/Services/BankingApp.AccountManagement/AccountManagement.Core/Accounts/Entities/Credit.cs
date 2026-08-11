using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.ValueObjects;
using BankingAppDDD.Domains.Abstractions.ValueObjects.Shared;
using System.ComponentModel.DataAnnotations;

namespace BankingAppDDD.Domains.Accounts.Entities
{
    public sealed class Credit : EntityBase, IAccountNonGenericRepo
    {
        private Credit()
        {
            //Amount = default!;
            //Description = string.Empty;
        }
        private Credit(Guid accountId, Amount amount, DateTime transactionDate, string description)
        {
            this.AccountId = accountId;
            this.Amount = amount;
            this.TransactionDate = transactionDate;
            this.Description = description;
            
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

        public static Credit Create(Guid accountId, Amount amount, DateTime transactionDate, string description)
        {
            return new Credit(accountId, amount, transactionDate, description);
        }

    }
}
