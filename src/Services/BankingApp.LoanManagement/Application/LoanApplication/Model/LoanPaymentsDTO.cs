
namespace BankingApp.LoanManagement.Application.LoanApplicationModels
{
    public class LoanPaymentsDTO
    {
        public Guid PaymentId { get; set; }
        public Guid LoanId { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
