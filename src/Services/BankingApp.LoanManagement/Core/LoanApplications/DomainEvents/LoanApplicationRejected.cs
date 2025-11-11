using BankingAppDDD.Domains.Abstractions.DomainEvents;
using BankingAppDDD.Domains.Abstractions.Models;

namespace BankingApp.LoanManagement.Core.LoanApplicationsDomainEvents
{
    public record class LoanApplicationRejected : DomainEvent
    {
        public Guid CustomerId { get; private set; }
        public LoanApplicationStatus LoanApplicationStatus { get; private set; }
        private LoanApplicationRejected(Guid customerId, LoanApplicationStatus loanApplicationStatus)
        {
            this.CustomerId = customerId;
            LoanApplicationStatus = loanApplicationStatus;
        }

        public static LoanApplicationRejected Create(Guid id, LoanApplicationStatus loanApplicationStatus)
        {
            if (id == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(id));
            return new LoanApplicationRejected(id, loanApplicationStatus);
        }
    }
}
