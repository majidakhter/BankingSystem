namespace BankingApp.PaymentProcessing.Model
{
    public class PaymentInitiationResult
    {
        public string OrderId { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string QrData { get; set; } = string.Empty;
        public string UpiId { get; set; } = string.Empty;
    }
}
