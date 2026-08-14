using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingAppDDD.Domains.Accounts.ValueObjects
{
    public sealed class Beneficiary : ValueObject
    {
        public string BeneficaryName { get; private set; }
        public int BeneficaryAccountNo { get; private set; }
        public string BeneficaryBankName { get; private set; }
        public string BeneficaryIfscCode { get; private set; }
        public static Beneficiary Create(string beneficaryName, int beneficaryAccountNo, string beneficaryBankName, string beneficaryIfscCode)
        {
            return new Beneficiary(beneficaryName, beneficaryAccountNo, beneficaryBankName, beneficaryIfscCode);
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return BeneficaryName;
            yield return BeneficaryAccountNo;
            yield return BeneficaryBankName;
            yield return BeneficaryIfscCode;
        }

        private Beneficiary(string beneficaryName, int beneficaryAccountNo, string beneficaryBankName, string beneficaryIfscCode)
        {
            this.BeneficaryName = beneficaryName;
            this.BeneficaryAccountNo = beneficaryAccountNo;
            this.BeneficaryBankName = beneficaryBankName;
            this.BeneficaryIfscCode = beneficaryIfscCode;
        }
    }
}