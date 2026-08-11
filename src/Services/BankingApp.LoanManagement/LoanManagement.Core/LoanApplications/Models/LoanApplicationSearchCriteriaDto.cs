using BankingAppDDD.Domains.Abstractions.Models;

namespace BankingAppDDD.Domains.LoanApplications.Models
{
    public class LoanApplicationSearchCriteriaDto
    {
        public string? LoanApplicationNumber { get; set; }
        public LoanApplicationStatus Status { get; set; }
        public DateOnly DecisionDate { get; set; }
        public decimal LoanAmount { get; set; }
        public Guid DecisionBy { get; set; }
    }
}
