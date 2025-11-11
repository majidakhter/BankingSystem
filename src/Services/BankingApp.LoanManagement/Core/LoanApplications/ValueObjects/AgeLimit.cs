using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingApp.LoanManagement.Core.LoanApplications.ValueObjects
{
    public sealed class AgeLimit : ValueObject
    {
        private readonly int age;

        public AgeLimit(int age)
        {
            this.age = age;
        }

        public static AgeLimit Create(DateOnly start, DateOnly end)
        {
            return new AgeLimit(end.Year - start.Year);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return age;
        }
        public static bool operator >(AgeLimit one, AgeLimit two) => one.CompareTo(two) > 0;

        public static bool operator <(AgeLimit one, AgeLimit two) => one.CompareTo(two) < 0;

        public static bool operator >=(AgeLimit one, AgeLimit two) => one.CompareTo(two) >= 0;

        public static bool operator <=(AgeLimit one, AgeLimit two) => one.CompareTo(two) <= 0;

        public int CompareTo(AgeLimit other) => age.CompareTo(other.age);
    }

    public static class AgeInYearsExtensions
    {
        public static AgeLimit Years(this int age) => new AgeLimit(age);
    }
}

