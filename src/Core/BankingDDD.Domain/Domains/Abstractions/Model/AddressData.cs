namespace BankingAppDDD.Domains.Abstractions.Models
{
    public record class AddressData(string street, string city, string state, string zipCode, string country);
}
