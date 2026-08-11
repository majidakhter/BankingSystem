namespace BankingAppDDD.PaymentProcessing.Model
{
    public record class PaymentResponse
    {
        public int TransactionNumber { get; set; }
        public string GatewayReference { get; set; }  // e.g., Bank UTR or Provider ID
        public TransactionStatus Status { get; set; }
        public string Message { get; set; }
    }
}
