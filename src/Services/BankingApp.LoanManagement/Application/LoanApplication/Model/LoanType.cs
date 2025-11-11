using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.Guards;

namespace BankingApp.LoanManagement.Application.LoanApplicationModels
{
    public class LoanType : Enumeration
    {

        public static LoanType Mortgage = new LoanType(1, nameof(Mortgage).ToLowerInvariant());
        public static LoanType Education = new LoanType(2, nameof(Education).ToLowerInvariant());
        public static LoanType Home = new LoanType(3, nameof(Home).ToLowerInvariant());
        public static LoanType Car = new LoanType(3, nameof(Car).ToLowerInvariant());
        public static LoanType Personal = new LoanType(3, nameof(Personal).ToLowerInvariant());
        public static LoanType Gold = new LoanType(3, nameof(Gold).ToLowerInvariant());

        public LoanType(LoanType t) : base(t.Id, t.Name)
        {
        }
        public LoanType()
        {

        }
        public LoanType(int id, string name)
         : base(id, name)
        {
        }
        public static IEnumerable<LoanType> List() =>
            new[] { Mortgage, Education, Home, Car, Personal, Gold };

        public static LoanType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new AccountDomainException($"Possible values for LoanType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static LoanType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new AccountDomainException($"Possible values for LoanType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
