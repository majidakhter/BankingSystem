using Autofac.Builder;
using BankingAppDDD.Domains.Abstractions.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApp.LoanManagement.Application.LoanApplicationModels
{
    public class LoanApplicationDTO
    {

        public Guid LoanId { get; set; }
        public string? LoanApplicationNumber { get; set; }
        public Guid CustomerId { get; set; }
        public int LoanTypeId { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int LoanNoOfYears { get; set; }
        public LoanApplicationStatus Status { get; set; }
        public string? Explanation { get; set; }
        public string? ScoringResult { get; set; }
        public DateOnly CustomerBirthDate { get; set; }
        public decimal CustomerMonthlyIncome {get;set;}
        public decimal PropertyValue { get; set; }
        public Guid DecisionBy { get; set; }
        public DateOnly DecisionDate { get; set; }
        public Guid RegisteredBy { get; set; }
        public DateOnly RegistrationDate { get; set; }
        public AddressDTO? PropertyAddress { get; set; }
    }
}
