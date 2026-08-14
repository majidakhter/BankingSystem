using System;
using System.Collections.Generic;
using BankingAppDDD.Domains.Accounts.Models;

namespace BankingAppDDD.Applications.Abstractions.IntegrationEvents.Transfer
{
    public sealed record FraudEvaluationCompletedIntegrationEvent(
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
        string Action, // "ALLOW", "TRIGGER_MFA_STEP_UP", "BLOCK_TRANSACTION"
        float MlRiskScore,
        float LlmRiskScore,
        float EnsembleScore,
        List<string> RiskFactors,
        string CorrelationId) : MassTransitIntegrationEvent(CorrelationId);
}
