namespace BankingAppDDD.Domains.Accounts.Models
{
    public enum TransferType
    {
        SameBank = 1,
        NEFT = 2,
        RTGS = 3,
        UPI = 4
    }

    public enum PaymentGatewayProvider
    {
        Internal = 0,
        RazorPay = 1,
        PhonePe = 2
    }

    public enum TransferStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2,
        PendingRetry = 3,
        Reversed = 4
    }
}
