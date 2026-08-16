namespace BankingAppDDD.Domains.Accounts.Models
{
    public enum TransferType
    {
        IMPS = 1,
        NEFT = 2,
        RTGS = 3
    }

    public enum PaymentMethodType
    {
        CreditDebitCard, 
        DigitalWallet,
        BankTransfer,
        BillPayment
    }
    public enum PaymentReceiverBusinessEntity
    {
        Merchant = 1,
        Biller = 2,
        Wallet = 3,
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
        Reversed = 4,
        PendingVerification = 5,
        Processing = 6,
        RequiresOtp = 7,
        Rejected = 8
    }
    public enum TransferToEntity
    {
        OwnBankAccount = 1,
        OtherBankAccount =2
        
    }
}
