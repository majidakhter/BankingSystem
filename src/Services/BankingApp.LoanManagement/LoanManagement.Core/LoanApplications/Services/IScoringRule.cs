
using BankingAppDDD.Domains.LoanApplications.Entities;

namespace BankingAppDDD.Domains.LoanApplications.Services
{
    public interface IScoringRule
    {
        bool IsSatisfiedBy(LoanApplication loanApplication);
        string Message { get; }
    }
}
