using BankingAppDDD.Domains.Accounts.Models;

namespace BankingAppDDD.Applications.Abstractions.IntegrationEvents.Transfer
{
    // 1. Initiated Event: Published as soon as transfer is initiated (Frontend gets immediate Transfer Initiated response)
    public sealed record FundTransferInitiatedIntegrationEvent(
        Guid TransactionId,
        Guid AccountId,
        int AccountNo,
        string senderBankIfscCode,
        Guid? DestinationAccountId,
        decimal Amount,
        string currencyCode,
        string Description,
        TransferType TransferType,
        TransferToEntity transferToEntity,
        PaymentGatewayProvider PaymentGateway,
        string? BeneficiaryAccountNo,
        string? receiverBankIfscCode,
        string CorrelationId) : MassTransitIntegrationEvent(CorrelationId);

    // 2. Debited Event: Published when sender account is debited (Listened to by Notification Engine, Fraud Detection)
    public sealed record AccountDebitedIntegrationEvent(
        Guid TransactionId,
        Guid AccountId,
        decimal Amount,
        string currencyCode,
        DateTime DebitedAt,
        string Description,
        string CorrelationId) : MassTransitIntegrationEvent(CorrelationId);

    // 3. Sent to Clearing Event: Published when request sent to Central Clearing Network / Payment Gateway
    public sealed record SentToClearingIntegrationEvent(
        Guid TransactionId,
        Guid AccountId,
        string senderBankIfscCode,
        decimal Amount,
        string currencyCode,
        TransferType TransferType,
        TransferToEntity transferToEntity,
        PaymentGatewayProvider PaymentGateway,
        string? BeneficiaryAccountNo,
        string? receiverBankIfscCode,
        string? remarks,
        DateTime SubmittedAt,
        string CorrelationId) : MassTransitIntegrationEvent(CorrelationId);

    // 4. Settled Event: Published when Clearing Network / Payment Gateway confirms money settled at beneficiary bank
    public sealed record FundTransferSettledIntegrationEvent(
        Guid TransactionId,
        Guid AccountId,
        decimal Amount,
        string currencyCode,
        string GatewayTransactionRef,
        DateTime SettledAt,
        string CorrelationId) : MassTransitIntegrationEvent(CorrelationId);

    // 5. Requested Event: Triggered for gateway processing worker
    public sealed record FundTransferRequestedIntegrationEvent(
        Guid TransactionId,
        Guid AccountId,
        Guid? DestinationAccountId,
        decimal Amount,
        string currencyCode,
        TransferType TransferType,
        TransferToEntity transferToEntity,
        PaymentGatewayProvider PaymentGateway,
        string? BeneficiaryAccountNo,
        string? senderBankIfscCode,
        string? DestinationBankIfscCode,
        string Description,
        string CorrelationId) : MassTransitIntegrationEvent(CorrelationId);

    // 6. Completed Event
    public sealed record FundTransferCompletedIntegrationEvent(
        Guid TransactionId,
        Guid AccountId,
        string? GatewayTransactionRef,
        string CorrelationId) : MassTransitIntegrationEvent(CorrelationId);

    // 7. Failed Event
    public sealed record FundTransferFailedIntegrationEvent(
        Guid TransactionId,
        Guid AccountId,
        decimal Amount,
        string currencyCode,
        string FailureReason,
        string CorrelationId) : MassTransitIntegrationEvent(CorrelationId);

    // 8. Reversal Requested Event
    public sealed record FundTransferReversalRequestedIntegrationEvent(
        Guid TransactionId,
        Guid AccountId,
        decimal Amount,
        string currencyCode,
        string ReversalReason,
        string CorrelationId) : MassTransitIntegrationEvent(CorrelationId);

    // 9. Reversed Event: Published when transaction reversal is completed (Listened to by Notification Engine & Risk)
    public sealed record FundTransferReversedIntegrationEvent(
        Guid TransactionId,
        Guid AccountId,
        decimal Amount,
        string currencyCode,
        string ReversalReason,
        DateTime ReversedAt,
        string CorrelationId) : MassTransitIntegrationEvent(CorrelationId);
}
