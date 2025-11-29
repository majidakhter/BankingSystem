
namespace BankingAppDDD.Domains.Abstractions.Guards
{
    public class BusinessRuleException(string message) : Exception(message) { }
}
