namespace BankingAppDDD.PaymentProcessing.Application.ProcessingPayment.IntegrationEvents;

public class PaymentFailed
{
    public Guid PaymentId { get; set; }
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public DateTime FailedAt { get; set; } = DateTime.UtcNow;
}
