

namespace BankingAppDDD.Domains.Abstractions.Guards
{
    public class AccountDomainException : DomainException
    {
        public AccountDomainException(string message) : base(message)
        {
        }
    }
}
