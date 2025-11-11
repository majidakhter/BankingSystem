

namespace BankingAppDDD.Domains.Abstractions.Guards
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {

        }
    }
}
