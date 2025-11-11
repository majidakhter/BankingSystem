using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingApp.LoanManagement.Core.LoanApplications.ValueObjects
{
    public sealed class Asset : ValueObject
    {
        public Amount Value { get; private set; }
        public Address Address { get; private set; }

        public static Asset Create(decimal value, Address address)
        {
            if (value == 0m)
                throw new ArgumentException("Value cannot be null");
            if (address == null)
                throw new ArgumentException("Address cannot be null");
          //  if (value <= Amount.Zero)
               // throw new ArgumentException("Property value must be higher than 0");

            return new Asset(value, address);
        }

        //To satisfy EF Core
        private Asset(decimal value, Address address)
        {
            Value = Amount.Create(value);
            Address = address;
        }
        private Asset() { }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            return new List<object> { Value, Address };
        }
    }
}
