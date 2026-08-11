using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.DebtInfos.ValueObjects;

namespace BankingAppDDD.Domains.DebtInfos.Entities
{
    public sealed class DebtorInfo : EntityBase, ILoanNonGenericRepository
    {
        public Guid IdentificationNumber { get; private set; } // this is basically mapped to customer guid

        public List<Debt> Debts { get; private set; }

        public static DebtorInfo Create(Guid identificationNumber, List<Debt> debts)
        {
            return new DebtorInfo(identificationNumber, debts);
        }
        private DebtorInfo(Guid identificationNumber, List<Debt> debts)
        {
            this.IdentificationNumber = identificationNumber;
            this.Debts = debts;
        }
        private DebtorInfo()
        {

        }
    }
}
