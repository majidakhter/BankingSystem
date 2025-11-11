using BankingApp.AccountManagement.Core.Accounts.ValueObjects;
using BankingAppDDD.Domains.Abstractions.Entities;
using System.Xml.Linq;

namespace BankingApp.AccountManagement.Core.Accounts.Entities
{
    public sealed class BeneficiaryGroup : EntityBase
    {
        private BeneficiaryGroup()
        {

        }
        private BeneficiaryGroup(Guid accountId, Beneficiary beneficiary, DateTime addedDate)
        {
            this.AccountId = accountId;
            this.Beneficiary = beneficiary;
            this.AddedDate = addedDate;
        }
        public Beneficiary Beneficiary { get; private set; }
        public Guid AccountId { get; private set; }
        public DateTime AddedDate { get; private set; }

        public static BeneficiaryGroup Create(BeneficiaryData beneficiary, Guid accountId, DateTime addedDate)
        {
            var (BeneficaryName, BeneficaryAccountNo, BeneficaryBankName) = beneficiary ?? throw new ArgumentNullException(nameof(beneficiary));
           
            Beneficiary beneficiaryData = Beneficiary.Create(BeneficaryName, BeneficaryAccountNo, BeneficaryBankName);

            return new BeneficiaryGroup(accountId, beneficiaryData, addedDate);
        }
    }
}
