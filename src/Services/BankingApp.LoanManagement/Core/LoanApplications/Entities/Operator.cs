using BankingApp.LoanManagement.Infrastructure.Repositories;
using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingApp.LoanManagement.Core.LoanApplicationsEntities
{
    public sealed class Operator : EntityBase, ILoanNonGenericRepository
    {
        public Amount CompetenceLevel { get; private set; }
        private Operator()
        {

        }
        public static Operator Create(Amount competenceLevel, Guid id)
        {
            if (id == Guid.Empty)
            {
                return new Operator(competenceLevel);
            }
            else
            {
                return new Operator(competenceLevel, id);
            }

        }

        //To satisfy EF Core
        private Operator(Amount competenceLevel, Guid id)
        {
            CompetenceLevel = competenceLevel;
            Id = id;
        }
        private Operator(Amount competenceLevel)
        {
            CompetenceLevel = competenceLevel;
        }

        public bool CanAccept(Amount loanLoanAmount) => loanLoanAmount <= CompetenceLevel;


    }

}
