using System;
using BankingAppDDD.Domains.Accounts.Models;

namespace BankingAppDDD.Applications.Abstractions.IntegrationEvents.Transfer
{
    public sealed record FundTransferSubmittedIntegrationEvent(
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
}
