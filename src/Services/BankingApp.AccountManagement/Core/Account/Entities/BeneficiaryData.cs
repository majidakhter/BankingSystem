using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApp.AccountManagement.Core.Accounts.Entities
{
    public record class BeneficiaryData(string beneficaryName, int beneficaryAccountNo, string beneficaryBankName);
}
