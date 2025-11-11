using BankingAppDDD.Domains.Abstractions.ValueObjects;
using System.Net;
using System.Xml.Linq;

namespace BankingApp.LoanManagement.Core.LoanApplications.ValueObjects
{
    public sealed class Customer : ValueObject
    {
        private Customer() { }
        public Amount MonthlyIncome { get; private set; }
        public DateOnly BirthDate { get; private set;}
        public Guid CustomerId { get; private set; }
        public static Customer Create(Amount monthlyIncome, DateOnly birthDate, Guid customerId)
        {
            if (monthlyIncome == null)
                throw new ArgumentException("Monthly income cannot be null");
            if (customerId == Guid.Empty)
                throw new ArgumentException("customerId cannot be null or empty");
            if (birthDate == default)
                throw new ArgumentException("Birthdate cannot be empty");
            return new Customer(monthlyIncome, birthDate, customerId);
        }

        //To satisfy EF Core
        private Customer(Amount monthlyIncome, DateOnly birthDate, Guid customerId)
        {
            MonthlyIncome = monthlyIncome;
            BirthDate = birthDate;
            CustomerId = customerId;
        }

        public AgeLimit AgeInYearsAt(DateOnly date) => AgeLimit.Create(BirthDate, date);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return MonthlyIncome;
            yield return BirthDate;
            yield return CustomerId;
        }
    }
}
