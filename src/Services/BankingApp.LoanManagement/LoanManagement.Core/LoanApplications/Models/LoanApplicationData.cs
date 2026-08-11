using BankingAppDDD.Domains.Abstractions.Models;

namespace BankingAppDDD.Domains.LoanApplications.Models
{
    public record class LoanApplicationData(LoanApplicationCustomerData customerData, AssetData assetData, LoanData loanData, int loanTypeId);
    public record class LoanApplicationCustomerData(Guid customerNationalIdentifier,
    DateOnly customerBirthdate,
    decimal customerMonthlyIncome);
    public record class AssetData(decimal PropertyValue,
    AddressData propertyAddress
);
    public record class LoanData(decimal loanAmount, int loanNumberOfYears, decimal percent);
   // public record class OperatorData(decimal competenceLevel, Guid underWriterId);

}
