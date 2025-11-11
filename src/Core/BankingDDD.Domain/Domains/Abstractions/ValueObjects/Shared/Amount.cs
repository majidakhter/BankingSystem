
using BankingAppDDD.Domains.Abstractions.Guards;
using BankingAppDDD.Domains.Extensions;

namespace BankingAppDDD.Domains.Abstractions.ValueObjects
{
    public sealed class Amount : ValueObject
    {

        public decimal Value { get; private set; }
        public static Amount Zero => Create(0);
        public static Amount Create(decimal number)
        {
            number.NotBeNegativeOrZero();
            if (number > 1000000)
            {
                throw new InvalidAmountException(number);
            }
            return new Amount(number);
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
        private Amount(decimal value)
        {
            //Value = value;
            Value = decimal.Round(value, 2, MidpointRounding.ToEven); 
        }
        public Amount MultiplyByPercent(Percent percent) => new Amount((this.Value * percent.Value) / 100M);
        public static implicit operator Amount(decimal v)
        {
            return new Amount(v);
        }

        public static Amount operator *(Amount a, Amount b)
        {
            return new Amount(a.Value * b.Value);
        }
        public static Amount operator +(Amount a, Amount b)
        {
            return new Amount(a.Value + b.Value);
        }

        public static Amount operator -(Amount a, Amount b)
        {
            return new Amount(a.Value - b.Value);
        }

        public static bool operator >(Amount a, Amount b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(Amount a, Amount b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >=(Amount a, Amount b)
        {
            return a.Value >= b.Value;
        }

        public static bool operator <=(Amount a, Amount b)
        {
            return a.Value <= b.Value;
        }
    }
}
