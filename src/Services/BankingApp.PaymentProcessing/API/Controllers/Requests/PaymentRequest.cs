using BankingAppDDD.PaymentProcessing.Model;

namespace BankingAppDDD.PaymentProcessing.API.Controllers.Requests;

public record class PaymentRequest
{
    public int TransactionNumber { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public int SourceAccountNo { get; set; }
    public int DestinationAccountNo { get; set; } // Bank Account Number or UPI ID (VPA)
    public string IfscCode { get; set; }          // Required for NEFT / RTGS
    public string Remarks { get; set; }
    public string CurrencyCode { get; init; } = "INR";
}

