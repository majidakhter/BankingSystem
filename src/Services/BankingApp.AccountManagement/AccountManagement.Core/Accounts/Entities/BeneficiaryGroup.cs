using BankingAppDDD.Domains.Accounts.Models;
using BankingAppDDD.Domains.Accounts.ValueObjects;
using BankingAppDDD.Domains.Abstractions.Entities;

namespace BankingAppDDD.Domains.Accounts.Entities
{
    public sealed class BeneficiaryGroup : EntityBase
    {
        private BeneficiaryGroup()
        {

        }
        private BeneficiaryGroup(Guid associatesourceaccountId, Beneficiary beneficiary, DateTime addedDate)
        {
            this.LoginUserAccountId = associatesourceaccountId;
            this.Beneficiary = beneficiary;
            this.AddedDate = addedDate;
            //this.BeneficiaryAccountId = Id;
        }
        public Beneficiary Beneficiary { get; private set; }
        public Guid LoginUserAccountId { get; private set; }
       // public Guid BeneficiaryAccountId { get; private set; }
        public DateTime AddedDate { get; private set; }

        public static BeneficiaryGroup Create(BeneficiaryData beneficiary, Guid associatesourceaccountId, DateTime addedDate)
        {
            var (BeneficaryName, BeneficaryAccountNo, BeneficaryBankName, BeneficaryIfscCode) = beneficiary ?? throw new ArgumentNullException(nameof(beneficiary));

            Beneficiary beneficiaryData = Beneficiary.Create(BeneficaryName, BeneficaryAccountNo, BeneficaryBankName, BeneficaryIfscCode);

            return new BeneficiaryGroup(associatesourceaccountId, beneficiaryData, addedDate);
        }
    }
}
