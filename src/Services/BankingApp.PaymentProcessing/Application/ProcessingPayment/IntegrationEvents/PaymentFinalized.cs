namespace BankingAppDDD.PaymentProcessing.Application.ProcessingPayment.IntegrationEvents;

public class PaymentFinalized
{
    public Guid PaymentId { get; set; }
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public DateTime FinalizedAt { get; set; } = DateTime.UtcNow;
}
