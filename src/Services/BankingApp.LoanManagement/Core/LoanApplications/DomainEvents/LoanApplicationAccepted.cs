using BankingAppDDD.Domains.Abstractions.DomainEvents;
using BankingAppDDD.Domains.Abstractions.Models;
namespace BankingApp.LoanManagement.Core.LoanApplicationsDomainEvents
{
    public record class LoanApplicationAccepted : DomainEvent
    {
        public Guid CustomerId { get; private set; }
        public LoanApplicationStatus LoanApplicationStatus { get; private set; }
        private LoanApplicationAccepted(Guid customerId, LoanApplicationStatus loanApplicationStatus)
        {
            this.CustomerId = customerId;
            LoanApplicationStatus = loanApplicationStatus;
        }

        public static LoanApplicationAccepted Create(Guid id, LoanApplicationStatus loanApplicationStatus)
        {
            if (id == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(id));
            return new LoanApplicationAccepted(id, loanApplicationStatus);
        }
    }
}
