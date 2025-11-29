using BankingAppDDD.Domains.Abstractions.Guards;

namespace BankingAppDDD.Domains.Abstractions.ValueObjects
{
    public sealed class BirthDate : ValueObject
    {
        private BirthDate() { }

        public DateOnly Value { get; private set; }

        public static BirthDate Create(DateOnly value)
        {
            // validations should be placed here instead of constructor
            if (value == default)
            {
                throw new DomainException($"BirthDate {value} cannot be null");
            }
            DateOnly dateonly= new DateOnly(2025, 1, 1);
            DateOnly minDateOfBirth = dateonly.AddYears(-115);
            DateOnly maxDateOfBirth = dateonly.AddYears(-15);

            // Validate the minimum age.
            if (value < minDateOfBirth || value > maxDateOfBirth)
            {
                throw new DomainException("The minimum age has to be 15 years.");
            }

            return new BirthDate { Value = value };
        }

        public static implicit operator DateOnly(BirthDate? value) => value?.Value ?? default;
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

    }
}
