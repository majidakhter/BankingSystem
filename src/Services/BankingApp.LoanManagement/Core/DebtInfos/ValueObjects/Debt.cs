using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingApp.LoanManagement.Core.DebtInfos.ValueObjects
{
    public sealed class Debt : ValueObject
    {
        public Amount Amount { get; private set; }

        public static Debt Create(decimal amount)
        {
            if (amount == 0m)
            {
                throw new ArgumentException("Amount cant be zero");
            }
            return new Debt(amount);
        }
        private Debt(decimal amount)
        {
            this.Amount = Amount.Create(amount);
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
        }
        private Debt()
        {

        }
    }
}
