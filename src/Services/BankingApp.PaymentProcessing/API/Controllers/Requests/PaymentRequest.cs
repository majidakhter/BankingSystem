namespace BankingAppDDD.PaymentProcessing.API.Controllers.Requests;

public record class PaymentRequest
{
    
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    // The secure single-use payment token passed directly from client-side SDK UI
    public string? BillId { get; set; }
    public string? ContactNumber { get; set; }
    public string GatewayProvider { get; set; } = "Razorpay"; // "Razorpay" or "PhonePe"
   
}

