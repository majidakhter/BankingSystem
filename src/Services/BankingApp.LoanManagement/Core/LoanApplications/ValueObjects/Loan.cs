using static System.Math;
using static System.Linq.Enumerable;
using BankingAppDDD.Domains.Abstractions.ValueObjects;
using BankingApp.LoanManagement.Application.LoanApplicationModels;

namespace BankingApp.LoanManagement.Core.LoanApplications.ValueObjects
{
    public sealed class Loan : ValueObject
    {
        public Amount LoanAmount { get; private set; }
        public int LoanNumberOfYears { get; private set; }
        public Percent InterestRate { get; private set; }

        public static Loan Create(Amount loanAmount, int loanNumberOfYears, Percent interestRate)
        {
            if (loanAmount == null)
                throw new ArgumentException("Loan amount cannot be null");
            if (interestRate.Value == 0m)
                throw new ArgumentException("Interest rate cannot be zero");
            //if (loanAmount <= Amount.Zero)
               // throw new ArgumentException("Loan amount must be grated than 0");
           // if (interestRate <= Percent.Zero)
                //throw new ArgumentException("Interest rate must be higher than 0");
            if (loanNumberOfYears <= 0)
                throw new ArgumentException("Loan number of years must be greater than 0");

            return new Loan(loanAmount, loanNumberOfYears, interestRate);
        }

        public Amount MonthlyInstallment()
        {
            var totalInstallments = LoanNumberOfYears * 12;

            var x = Range(1, totalInstallments).Sum(
                i => Pow(1.0 + (double)InterestRate.Value / 100 / 12, -1 * i));

            return Amount.Create(LoanAmount.Value / Convert.ToDecimal(x));
        }

        public DateOnly LastInstallmentsDate() => SysTime.Today().AddYears(LoanNumberOfYears);



        //To Satisfy EF Core
        private Loan(Amount loanAmount, int loanNumberOfYears, Percent interestRate)
        {
            LoanAmount = loanAmount;
            LoanNumberOfYears = loanNumberOfYears;
            InterestRate = interestRate;
        }
        private Loan() { }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            return new List<object>
            {
            LoanAmount,
            LoanNumberOfYears,
            InterestRate
            };
        }
    }
}
