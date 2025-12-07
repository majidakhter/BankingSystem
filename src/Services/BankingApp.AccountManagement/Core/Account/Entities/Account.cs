using BankingApp.AccountManagement.Core.Accounts.DomainEvents;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingApp.AccountManagement.Core.Accounts.Entities
{
    public sealed class Account : EntityBase, IAggregateRoot, IAccountNonGenericRepo
    {
        public Guid CustomerId { get; private set; }
        public int AccountNo { get; private set; }
        public AccountType AccountType { get; private set; }
        public int AccountTypeId { get; private set; }
        public int AccountStatusId { get; private set; }
        public DateTime? DateAdded { get; private set; }
        public DateTime? CloasedDate { get; private set; }
        public DateTime? AccountUpdatedDate { get; private set; }

        private List<Credit> _credits;

        private List<Debit> _debits;

        private List<BeneficiaryGroup> _beneficiaries;
        public IReadOnlyCollection<Credit> Credits => _credits;
        public IReadOnlyCollection<Debit> Debits => _debits;
        public IReadOnlyCollection<BeneficiaryGroup> BeneficiaryGroups => _beneficiaries;
        public AccountStatus AccountStatus { get; private set; }
        public AccountStatus GetAccountStatus => AccountStatus.From(_accountStatusId);

        private int _accountStatusId;
        protected Account()
        {
            _credits = new List<Credit>();
            _debits = new List<Debit>();
            _beneficiaries = new List<BeneficiaryGroup>();
        }
        private Account(Guid customerId, int accountTypeId)
        {
            //only mandatory fields are required in constructor
            var accountTypeIdEnumEnums = AccountType.List().FirstOrDefault(x => x.Id == accountTypeId);
            var @event = AccountAddedDomainEvent.Create(
                Id,
                customerId,
                accountTypeIdEnumEnums.Id);


            AddDomainEvent(@event);
            Apply(@event);
        }
        //Factory method to restore state
        public static Account Create(Guid customerId, int accountTypeId)
        {
            var account = new Account(customerId, accountTypeId);

            return account;
        }
        private void Apply(AccountAddedDomainEvent @event)
        {
            Id = @event.AccountId;
            CustomerId = @event.CustomerId;
            this.AccountTypeId = @event.AccountTypeId;
            DateAdded = @event.Timestamp;
            this.AccountStatusId = AccountStatus.Opened.Id;
            this._credits = new List<Credit>();
            this._debits = new List<Debit>();
            this._beneficiaries = new List<BeneficiaryGroup>();
        }
        public Credit Deposit(decimal deposiedAmount)
        {
            Amount amount = Amount.Create(deposiedAmount);
            Credit credit = Credit.Create(Id, amount, DateTime.UtcNow, "Deposit");
            _credits.Add(credit);
            return credit;
        }
        public Debit Withdraw(decimal withdrawalAmount)
        {
            Amount amount = Amount.Create(withdrawalAmount);
            Amount balance = GetBalance();
            if (amount > balance)
                throw new Exception($"The Account {Id} does not have sufficient funds to withdraw {amount} current balance {balance}");
            Debit debit = Debit.Create(Id, amount, DateTime.UtcNow, "withdraw");
            _debits.Add(debit);
            return debit;
        }

        public void Close(Guid customerId)
        {
            if (GetBalance() > 0)
                throw new Exception($"The account {Id} can not be closed because it has funds.");
            var @event = AccountClosedDomainEvent.Create(
                 Id,
                 customerId);

            AddDomainEvent(@event);
            Apply(@event);
        }
        private void Apply(AccountClosedDomainEvent @event)
        {
            Id = @event.AccountId;
            CustomerId = @event.CustomerId;
            this.AccountStatusId = AccountStatus.Closed.Id;
            CloasedDate = @event.Timestamp;
        }
        public BeneficiaryGroup AddBeneficiary(BeneficiaryData beneficiary)
        {
            BeneficiaryGroup group = BeneficiaryGroup.Create(beneficiary, Id, DateTime.UtcNow);
            _beneficiaries.Add(group);
            return group;
        }

        /*public Amount GetCurrentBalance() // Balance check
        {
            Amount totalAmount = Transactions.GetCurrentBalance();
            return totalAmount;
        }
        public ITransaction GetLastTransaction()
        {
            ITransaction transaction = Transactions.GetLastTransaction();
            return transaction;
        }*/
        public Amount GetBalance()
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
        }
    }
}
