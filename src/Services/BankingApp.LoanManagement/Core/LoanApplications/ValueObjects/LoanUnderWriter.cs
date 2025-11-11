using BankingAppDDD.Domains.Abstractions.ValueObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BankingApp.LoanManagement.Core.LoanApplications.ValueObjects
{
    public sealed class LoanUnderWriter : ValueObject
    {
        public DateOnly DecisionDate { get; private set; }
        public Guid DecisionBy { get; private set; }

        //Decision By
        private LoanUnderWriter()
        {

        }
        [JsonConstructor]
        private LoanUnderWriter(DateOnly decisionDate, Guid decisionBy)
        {
            DecisionDate = decisionDate;
            DecisionBy = decisionBy;
        }

        // To Satisfy EF Core
        public static  LoanUnderWriter Create(DateOnly decisionDate, Guid decisionBy)
        {
            return new LoanUnderWriter(decisionDate, decisionBy);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return DecisionDate;
            yield return DecisionBy;
        }
    }
}
