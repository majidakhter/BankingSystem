using System;
using System.Collections.Generic;

namespace BankingAppDDD.Domains.Accounts.Models
{
    public class UserAccountDTO
    {
        public Guid AccountId { get; set; }
        public string? UserFullName { get; set; }
        public int AccountNo { get; set; }
        public int AccountTypeId { get; set; }
        public int AccountStatusId { get; set; }
        public decimal CurrentBalance { get; set; }
        public IEnumerable<CreditDTO> TransactionDetail { get; set; } = new List<CreditDTO>();
        public IEnumerable<BeneficiaryDTO> BeneficiariesDetail { get; set; } = new List<BeneficiaryDTO>();
    }

    public class CreditDTO
    {
        public int TransactionNumber { get; set; }
        public decimal TransactionAmount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
