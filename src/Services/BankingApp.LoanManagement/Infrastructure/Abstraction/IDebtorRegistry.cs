
using BankingApp.LoanManagement.Core.LoanApplications.ValueObjects;

namespace BankingApp.LoanManagement.Infrastructure.Abstraction
{
    public interface IDebtorRegistry
    {
        bool IsRegisteredDebtor(Customer customer);
    }
}
