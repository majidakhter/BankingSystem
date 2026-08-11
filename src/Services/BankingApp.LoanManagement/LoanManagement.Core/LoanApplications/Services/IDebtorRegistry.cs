
using BankingAppDDD.Domains.LoanApplications.ValueObjects;

namespace BankingAppDDD.Domains.LoanApplications.Services
{
    public interface IDebtorRegistry
    {
        bool IsRegisteredDebtor(Customer customer);
    }
}
