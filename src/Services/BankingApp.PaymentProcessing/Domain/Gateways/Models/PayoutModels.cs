
namespace BankingAppDDD.PaymentProcessing.Domain.Gateways.Models
{
    public record PayoutRequest(
        Guid TransactionId,
        Guid AccountId,
        decimal Amount,
        TransferType TransferType,
        PaymentGatewayProvider PaymentGateway,
        string? BeneficiaryAccountNo,
        string? IfscCode,
        string Description);

    public record PayoutResponse(
        bool IsSuccess,
        string? GatewayTransactionRef,
        string? ErrorMessage);
}
