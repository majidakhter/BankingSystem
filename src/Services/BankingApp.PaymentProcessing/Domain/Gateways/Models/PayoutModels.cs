using BankingAppDDD.Domains.Accounts.Models;

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
        string? UpiId,
        string? DestinationBankName,
        string Description);

    public record PayoutResponse(
        bool IsSuccess,
        string? GatewayTransactionRef,
        string? ErrorMessage);
}
