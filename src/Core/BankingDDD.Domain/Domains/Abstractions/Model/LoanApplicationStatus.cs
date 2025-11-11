

namespace BankingAppDDD.Domains.Abstractions.Models
{
    public enum LoanApplicationStatus
    {
        New,
        Accepted,
        Rejected
    }
   /* public class LoanApplicationStatus : Enumeration
    {

        public static LoanApplicationStatus None = new LoanApplicationStatus(1, nameof(None).ToLowerInvariant());
        public static LoanApplicationStatus New = new LoanApplicationStatus(2, nameof(New).ToLowerInvariant());
        public static LoanApplicationStatus Accepted = new LoanApplicationStatus(3, nameof(Accepted).ToLowerInvariant());
        public static LoanApplicationStatus Rejected = new LoanApplicationStatus(3, nameof(Rejected).ToLowerInvariant());

        public LoanApplicationStatus(LoanApplicationStatus t) : base(t.Id, t.Name)
        {
        }
        public LoanApplicationStatus()
        {

        }
        public LoanApplicationStatus(int id, string name)
         : base(id, name)
        {
        }
        public static IEnumerable<LoanApplicationStatus> List() =>
            new[] { None, New, Accepted, Rejected };

        public static LoanApplicationStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new AccountDomainException($"Possible values for LoanApplicationStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static LoanApplicationStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new AccountDomainException($"Possible values for LoanApplicationStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }*/
}
