using BankingApp.LoanManagement.Core.LoanApplications.ValueObjects;
using BankingApp.LoanManagement.Core.LoanApplicationsEntities;
using BankingApp.LoanManagement.Infrastructure.Abstraction;
using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingApp.LoanManagement.Infrastructure.Factory
{
    public class ScoringRules(IList<IScoringRule> rules)
    {
        public ScoringResult Evaluate(LoanApplication loanApplication)
        {
            var brokenRules = rules
                .Where(r => !r.IsSatisfiedBy(loanApplication))
                .ToList();

            return brokenRules.Any() ?
                ScoringResult.Low(brokenRules.Select(r => r.Message).ToArray()) : ScoringResult.High();
        }
    }
    public class LoanAmountMustBeLowerThanPropertyValue : IScoringRule
    {
        public bool IsSatisfiedBy(LoanApplication loanApplication)
        {
            return loanApplication.Loan.LoanAmount < loanApplication.Asset.Value;
        }

        public string Message => "Property value is lower than loan amount.";
    }

    public class CustomerAgeAtTheDateOfLastInstallmentMustBeBelow65 : IScoringRule
    {
        public bool IsSatisfiedBy(LoanApplication loanApplication)
        {
            var lastInstallmentDate = loanApplication.Loan.LastInstallmentsDate();
            return loanApplication.Customer.AgeInYearsAt(lastInstallmentDate) < 65.Years();
        }

        public string Message => "Customer age at last installment date is above 65.";
    }

    public class InstallmentAmountMustBeLowerThen15PercentOfCustomerIncome : IScoringRule
    {
        public bool IsSatisfiedBy(LoanApplication loanApplication)
        {
            decimal val = loanApplication.Customer.MonthlyIncome.Value * 15.Percent().Value;
            return loanApplication.Loan.MonthlyInstallment()
                   < loanApplication.Customer.MonthlyIncome.Value * 15.Percent().Value;
        }

        public string Message => "Installment is higher than 15% of customer's income.";
    }

    public class CustomerIsNotARegisteredDebtor : IScoringRule
    {
        private readonly IDebtorRegistry debtorRegistry;

        public CustomerIsNotARegisteredDebtor(IDebtorRegistry debtorRegistry)
        {
            this.debtorRegistry = debtorRegistry;
        }

        public bool IsSatisfiedBy(LoanApplication loanApplication)
        {
            return !debtorRegistry.IsRegisteredDebtor(loanApplication.Customer);
        }
        public string Message => "Customer is registered in debtor registry";

    }

}
