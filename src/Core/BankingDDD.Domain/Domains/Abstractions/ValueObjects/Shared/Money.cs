using System;
using System.Collections.Generic;
using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingAppDDD.Domains.Abstractions.ValueObjects.Shared
{
    public sealed class Money : ValueObject
    {
        public Amount Amount { get; private set; }
        public Currency Currency { get; private set; }

        private Money() 
        {
            Amount = Amount.Zero;
            Currency = Currency.Rupee;
        }

        private Money(Amount amount, Currency currency)
        {
            Amount = amount ?? throw new ArgumentNullException(nameof(amount));
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        }

        public static Money Create(decimal amount, string currencyCode = "INR")
        {
            var amt = Amount.Create(amount);
            var curr = string.IsNullOrWhiteSpace(currencyCode) ? Currency.Rupee : Currency.Create(currencyCode);
            return new Money(amt, curr);
        }

        public static Money Create(Amount amount, Currency currency)
        {
            return new Money(amount, currency);
        }

        public static Money Zero(Currency? currency = null) => Create(0, currency?.Code ?? "INR");

        public Money Add(Money other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (Currency != other.Currency)
                throw new InvalidOperationException($"Cannot add money of different currencies: {Currency} and {other.Currency}");
            return Create(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (Currency != other.Currency)
                throw new InvalidOperationException($"Cannot subtract money of different currencies: {Currency} and {other.Currency}");
            return Create(Amount - other.Amount, Currency);
        }

        public override string ToString() => $"{Currency.Code} {Amount.Value}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}
