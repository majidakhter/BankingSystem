using BankingAppDDD.Domains.Abstractions.ValueObjects;
using BankingAppDDD.Domains.LoanApplications.Models;
using Newtonsoft.Json;

namespace BankingAppDDD.Domains.LoanApplications.ValueObjects
{
    public sealed class ScoringResult : ValueObject
    {
        public ApplicantScore? Score { get; private set; }
        public string Explanation { get; private set; }

        [JsonConstructor]
        private ScoringResult(ApplicantScore? score, string explanation)
        {
            Score = score;
            Explanation = explanation;
        }

        //To satisfy EF Core
        public static ScoringResult Create(ApplicantScore? score, string explanation)
        {
            return new ScoringResult(score, explanation);
        }

        public static ScoringResult High() =>
            new(ApplicantScore.High, string.Empty);


        public static ScoringResult Low(string[] messages)
            => new(ApplicantScore.Low, string.Join(Environment.NewLine, messages));


        public bool IsLow() => Score == ApplicantScore.Low;
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Score;
            yield return Explanation;
        }
    }
}
