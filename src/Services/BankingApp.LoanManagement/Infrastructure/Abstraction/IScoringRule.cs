
using BankingApp.LoanManagement.Core.LoanApplicationsEntities;

namespace BankingApp.LoanManagement.Infrastructure.Abstraction
{
    public interface IScoringRule
    {
        bool IsSatisfiedBy(LoanApplication loanApplication);
        string Message { get; }
    }
}
