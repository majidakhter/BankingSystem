

using BankingAppDDD.Domains.Extensions;

namespace BankingAppDDD.Domains.Abstractions.ValueObjects
{
    public sealed class PhoneNumber : ValueObject
    {
        private PhoneNumber()
        {

        }
        public string Value { get; private set; } = default!;
        private PhoneNumber(string value)
        {
            Value = value;
        }
        public static PhoneNumber Create(string value)
        {
            value.NotBeNull();
            value.NotBeInvalidPhoneNumber();
            return new PhoneNumber(value);
        }

        public static implicit operator string(PhoneNumber? phoneNumber) => phoneNumber?.Value ?? string.Empty;
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
        public void Deconstruct(out string value) => value = Value;
    }
}
