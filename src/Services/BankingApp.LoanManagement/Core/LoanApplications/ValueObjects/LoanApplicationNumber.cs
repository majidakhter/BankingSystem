using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingApp.LoanManagement.Core.LoanApplications.ValueObjects
{
    public sealed class LoanApplicationNumber : ValueObject
    {
        public string Number { get; private set; }
        public LoanApplicationNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Loan application number cannot be null or empty string");
            Number = number;
        }


        public static LoanApplicationNumber NewNumber() => new LoanApplicationNumber(Guid.NewGuid().ToString());

        public static LoanApplicationNumber Create(string number) => new LoanApplicationNumber(number);

        public static implicit operator string(LoanApplicationNumber number) => number.Number;


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Number;
        }
    }
}
