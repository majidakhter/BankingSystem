
using BankingApp.LoanManagement.Infrastructure.Abstraction;

namespace BankingApp.LoanManagement.Infrastructure.Factory
{
    public class ScoringRulesFactory(IDebtorRegistry debtorRegistry)
    {
        public ScoringRules DefaultSet => new ScoringRules(new List<IScoringRule>
    {
        new LoanAmountMustBeLowerThanPropertyValue(),
        new CustomerAgeAtTheDateOfLastInstallmentMustBeBelow65(),
        new InstallmentAmountMustBeLowerThen15PercentOfCustomerIncome(),
        new CustomerIsNotARegisteredDebtor(debtorRegistry)
    });
    }
}
