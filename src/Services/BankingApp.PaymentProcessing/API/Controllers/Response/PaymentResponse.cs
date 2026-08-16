namespace BankingAppDDD.PaymentProcessing.Model
{
    public record class PaymentResponse
    {
        public string TransactionNumber { get; set; }
        public TransactionStatus Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}
