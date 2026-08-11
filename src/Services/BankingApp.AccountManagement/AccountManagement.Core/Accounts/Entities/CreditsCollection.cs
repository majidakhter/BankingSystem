
using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingAppDDD.Domains.Accounts.Entities
{
    public sealed class CreditsCollection : List<Credit>
    {
        /// <summary>
        ///     Gets Total amount.
        /// </summary>
        /// <returns>Positive amount.</returns>
        public Amount GetTotal()
        {
            if (this.Count == 0)
            {
                return Amount.Create(0);
            }

            Amount total = Amount.Create(0);

            return this.Aggregate(total, (current, credit) =>
                Amount.Create(current.Value + credit.Amount.Value));
        }
    }
}
