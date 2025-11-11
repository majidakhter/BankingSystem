using BankingAppDDD.Domains.Abstractions.Guards;
using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingApp.LoanManagement.Core.LoanApplications.ValueObjects
{
    public sealed class CreditLimit : ValueObject
    {
        public decimal Amount { get; private set; }

        public static CreditLimit Create(decimal creditLimit)
        {
            if (creditLimit <= 0)
                throw new BusinessRuleException("The customer credit limit must be greater than zero.");

            return new CreditLimit(creditLimit);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
        }

        private CreditLimit(decimal creditLimit) => Amount = creditLimit;
    }
}
