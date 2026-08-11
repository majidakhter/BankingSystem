using BankingAppDDD.Domains.Abstractions.Guards;
using BankingAppDDD.Domains.Abstractions.ValueObjects.Shared;
using BankingAppDDD.Domains.Extensions;
using System;
using System.Collections.Generic;

namespace BankingAppDDD.Domains.Abstractions.ValueObjects
{
    public sealed class Amount : ValueObject
    {
        public decimal Value { get; private set; }
        public static Amount Zero => new Amount(0);

        public static Amount Create(decimal number)
        {
            number.NotBeNegative();
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

        public Amount Subtract(Amount debit) =>
            new Amount(Math.Round(this.Value - debit.Value, 2));
    }
}
