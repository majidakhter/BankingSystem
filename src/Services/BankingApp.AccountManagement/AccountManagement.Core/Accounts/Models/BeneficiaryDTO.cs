using System;

namespace BankingAppDDD.Domains.Accounts.Models
{
    public class BeneficiaryDTO
    {
        public Guid Id { get; set; }
        public Guid LoginUserAccountId { get; set; }
        public string BeneficaryName { get; set; } = string.Empty;
        public int BeneficaryAccountNo { get; set; }
        public string BeneficaryBankName { get; set; } = string.Empty;
        public string BeneficaryIfscCode { get; set; } = string.Empty;
        public DateTime AddedDate { get; set; }
    }
}
