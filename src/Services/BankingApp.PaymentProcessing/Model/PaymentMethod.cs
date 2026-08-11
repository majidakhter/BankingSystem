namespace BankingAppDDD.PaymentProcessing.Model
{
    public enum PaymentMethod
    {
        NEFT,
        RTGS, 
        UPI
    }
    public enum TransactionStatus 
    { 
        Pending, 
        Success, 
        Failed 
    }
}
