using BankingAppDDD.Common.Helpers;
using BankingAppDDD.Domains.Accounts.DomainEvents;
using BankingAppDDD.Domains.Accounts.Models;
using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingAppDDD.Domains.Accounts.Entities
{
    public sealed class Account : EntityBase, IAggregateRoot, IAccountNonGenericRepo
    {
        public Guid UserId { get; private set; }
        public Guid KeycloakUserId { get; private set; }
        public int AccountNo { get; private set; }
        public int AccountTypeId { get; private set; }
        public int AccountStatusId { get; private set; }
        public DateTime? DateAdded { get; private set; }
        public DateTime? ClosedDate { get; private set; }
        public DateTime? AccountUpdatedDate { get; private set; }
        public CreditsCollection CreditsCollection { get; } = new CreditsCollection();
        public DebitsCollection DebitsCollection { get; } = new DebitsCollection();

        private List<BeneficiaryGroup> _beneficiaries = new List<BeneficiaryGroup>();
        //public IReadOnlyCollection<Credit> Credits => _credits;
        //public IReadOnlyCollection<Debit> Debits => _debits;
        //public IReadOnlyCollection<BeneficiaryGroup> BeneficiaryGroups => _beneficiaries;
        public AccountStatus AccountStatus { get; private set; }
        public AccountStatus GetAccountStatus => AccountStatus.From(_accountStatusId);

        private int _accountStatusId;
        protected Account()
        {
           
        }
        private Account(Guid userId, Guid keycloakUserId, int accountTypeId)
        {
            //only mandatory fields are required in constructor
            var accountTypeIdEnumEnums = AccountType.List().FirstOrDefault(x => x.Id == accountTypeId);
            var @event = AccountAddedDomainEvent.Create(
                Id,
                userId,
                keycloakUserId,
                accountTypeIdEnumEnums!.Id);


            AddDomainEvent(@event);
            Apply(@event);
        }
        //Factory method to restore state
       
        public static Account Create(Guid userId, Guid keycloakUserId, int accountTypeId)
        {
            
            var account = new Account(userId,keycloakUserId, accountTypeId);
            //account.Deposit(amount, "Deposited");
            return account;
        }
        private void Apply(AccountAddedDomainEvent @event)
        {
            Id = @event.AccountId;
            UserId = @event.UserId;
            KeycloakUserId = @event.KeycloakUserId;
            AccountNo = AccountNumberGenerator.GenerateDynamicAccountNumber(@event.UserId);
            AccountTypeId = @event.AccountTypeId;
            DateAdded = @event.Timestamp;
            AccountStatusId = AccountStatus.Opened.Id;
        }
        public Credit Deposit(Guid beneficiaryAccountId, decimal amountToDeposit, string description)
        {
            Amount amount = Amount.Create(amountToDeposit);
            Credit credit = Credit.Create(beneficiaryAccountId, amount, DateTime.UtcNow, description);
            CreditsCollection.Add(credit);
            var @event = AccountCreditedDomainEvent.Create(
                beneficiaryAccountId,
                amount,
                DateTime.UtcNow,
                description);
            AddDomainEvent(@event);
            return credit;
        }
        

        public Debit Withdraw(Guid accountId, decimal withdrawalAmount, string description)
        {
            Amount amount = Amount.Create(withdrawalAmount);
            Amount balance = GetCurrentBalance();
            if (amount > balance)
                throw new Exception($"The Account {accountId} does not have sufficient funds to withdraw {amount} current balance {balance}");
            Debit debit = Debit.Create(accountId, amount, DateTime.UtcNow, description);
            DebitsCollection.Add(debit);
            var @event = AccountDebitedDomainEvent.Create(
                accountId,
                amount,
                DateTime.UtcNow,
                description);
            AddDomainEvent(@event);
            return debit;
        }

        

        public void Close(Guid userId, Guid accountId)
        {
            if (GetCurrentBalance() > 0)
                throw new Exception($"The account {Id} can not be closed because it has funds.");
            var @event = AccountClosedDomainEvent.Create(
                 accountId,
                 userId);

            AddDomainEvent(@event);
            Apply(@event);
        }
        private void Apply(AccountClosedDomainEvent @event)
        {
            Id = @event.AccountId;
            UserId = @event.UserId;
            this.AccountStatusId = AccountStatus.Closed.Id;
            ClosedDate = @event.Timestamp;
        }
        public BeneficiaryGroup AddBeneficiary(BeneficiaryData beneficiary, Guid associateaccountId)
        {
            BeneficiaryGroup group = BeneficiaryGroup.Create(beneficiary, associateaccountId, DateTime.UtcNow);
            _beneficiaries.Add(group);
            return group;
        }

        /* public ITransaction GetLastTransaction()
         {
             return Transactions.GetLastTransactionByDate(); ;
         }
         public void AddTransaction(decimal amount, string description)
         {
             Transactions.Add(new Transaction { Amount = amount, TransactionDate = DateTime.Now, Description = description });
         }
          public Amount GetCurrentBalance()
          {
              Amount balance = 0;
              foreach (var c in Credits)
              {
                  balance = balance + c.Amount;
              }

              foreach (var d in Debits)
              {
                  balance = balance - d.Amount;
              }
              return balance;
          }*/

        public Amount GetCurrentBalance()
        {
            Amount totalCredits = this.CreditsCollection
            .GetTotal();

            Amount totalDebits = this.DebitsCollection
                .GetTotal();

            Amount totalAmount = totalCredits
                .Subtract(totalDebits);

            return totalAmount;
        }
    }
}
