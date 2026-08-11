namespace BankingAppDDD.PaymentProcessing.Domain;

public enum PaymentCancellationReason
{
    ProcessmentError = 0,
    GatewayTimeout = 1,
    InsufficientFunds = 2,
    InvalidBeneficiary = 3,
    NetworkDown = 4,
    ReversalExecuted = 5
}