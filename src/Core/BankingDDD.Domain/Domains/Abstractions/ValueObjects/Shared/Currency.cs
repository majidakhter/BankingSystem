using BankingAppDDD.Domains.Abstractions.Guards;
using BankingAppDDD.Domains.Abstractions.ValueObjects;


namespace BankingAppDDD.Domains.Abstractions.ValueObjects.Shared
{
    public sealed class Currency : ValueObject
    {
        private Currency() { }

        public string Code { get; private set; }

        public static Currency Create(string code)
        {
            // validations should be placed here instead of constructor
            if (string.IsNullOrEmpty(code))
            {
                throw new DomainException($"Currency Code {code} cannot be null");
            }
            return new Currency { Code = code };
        }
        public override bool Equals(object? obj) =>
       obj is Currency o && this.Equals(o);

        public bool Equals(Currency other) => this.Code == other.Code;

        public override int GetHashCode() =>
            HashCode.Combine(this.Code);

        public static bool operator ==(Currency left, Currency right) => left.Equals(right);

        public static bool operator !=(Currency left, Currency right) => !(left == right);

        /// <summary>
        ///     Dollar.
        /// </summary>
        /// <returns>Currency.</returns>
        public static readonly Currency Dollar = Currency.Create("USD");

        /// <summary>
        ///     Euro.
        /// </summary>
        /// <returns>Currency.</returns>
        public static readonly Currency Euro =  Currency.Create("EUR");

        /// <summary>
        ///     British Pound.
        /// </summary>
        /// <returns>Currency.</returns>
        public static readonly Currency BritishPound = Currency.Create("GBP");

        /// <summary>
        ///     Canadian Dollar.
        /// </summary>
        /// <returns>Currency.</returns>
        public static readonly Currency Canadian = Currency.Create("CAD");

        /// <summary>
        ///     Brazilian Real.
        /// </summary>
        /// <returns>Currency.</returns>
        public static readonly Currency Real = Currency.Create("BRL");

        /// <summary>
        ///     Swedish Krona.
        /// </summary>
        /// <returns>Currency.</returns>
        public static readonly Currency Krona = Currency.Create("SEK");
        /// <summary>
        /// 
        ///<summary>
        /// Indian Rupee
        /// </summary>
        /// <returns>Currency.</returns>.
        public static readonly Currency Rupee = Currency.Create("INR");



        public override string ToString() => this.Code;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Code;
        }

    }
}
