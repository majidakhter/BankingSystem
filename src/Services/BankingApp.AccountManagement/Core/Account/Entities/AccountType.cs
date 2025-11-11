
using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.Guards;

namespace BankingApp.AccountManagement.Core.Accounts.Entities
{
    public class AccountType : Enumeration
    {

        public static AccountType Savings = new AccountType(1, nameof(Savings).ToLowerInvariant());
        public static AccountType Current = new AccountType(2, nameof(Current).ToLowerInvariant());
        public static AccountType Loan = new AccountType(3, nameof(Loan).ToLowerInvariant());
        public static AccountType PPF = new AccountType(4, nameof(PPF).ToLowerInvariant());
        public AccountType(AccountType t) : base(t.Id, t.Name)
        {
        }
        public AccountType()
        {

        }
        public AccountType(int id, string name)
         : base(id, name)
        {
        }

        public static IEnumerable<AccountType> List() =>
            new[] { Savings, Current, Loan, PPF };

        public static AccountType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new AccountDomainException($"Possible values for AccountType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static AccountType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new AccountDomainException($"Possible values for AccountType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
