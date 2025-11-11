using BankingApp.LoanManagement.Core.DebtInfos.Entities;
using BankingApp.LoanManagement.Core.LoanApplications.ValueObjects;
using BankingApp.LoanManagement.Infrastructure.Abstraction;
using BankingApp.LoanManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

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