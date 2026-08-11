using BankingApp.LoanManagement.Infrastructure.Repositories;
using BankingAppDDD.Domains.DebtInfos.Entities;
using BankingAppDDD.Domains.LoanApplications.Services;
using BankingAppDDD.Domains.LoanApplications.ValueObjects;

namespace BankingApp.LoanManagement.Infrastructure.Services;

public class DebtorRegistry : IDebtorRegistry
{
    private readonly ILoanRepository<DebtorInfo> _repository;
    public DebtorRegistry(ILoanRepository<DebtorInfo> repository)
    {
        _repository = repository;
    }
    public bool IsRegisteredDebtor(Customer customer)
    {
        var debtorInfo =  _repository.GetAll();
        if (debtorInfo != null)
        {
            var data = debtorInfo.Where(x => x.IdentificationNumber == customer.CustomerId).FirstOrDefault();
            if(data !=null && data!.Debts.Any())
            {
                return true;
            }
            return false;
        }
        return false;
    }


   
}