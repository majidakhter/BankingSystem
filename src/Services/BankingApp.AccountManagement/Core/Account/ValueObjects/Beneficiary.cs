using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingApp.AccountManagement.Core.Accounts.ValueObjects
{
    public sealed class Beneficiary : ValueObject
    {
        public string BeneficaryName { get; private set; }
        public int BeneficaryAccountNo { get; private set; }
        public string BeneficaryBankName { get; private set; }
        public static Beneficiary Create(string beneficaryName, int beneficaryAccountNo, string beneficaryBankName)
        {
            return new Beneficiary(beneficaryName, beneficaryAccountNo, beneficaryBankName);
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return BeneficaryName;
            yield return BeneficaryAccountNo;
            yield return BeneficaryBankName;
        }

        private Beneficiary(string beneficaryName, int beneficaryAccountNo, string beneficaryBankName)
        {
            this.BeneficaryName = beneficaryName;
            this.BeneficaryAccountNo = beneficaryAccountNo;
            this.BeneficaryBankName = beneficaryBankName;
        }
    }
}