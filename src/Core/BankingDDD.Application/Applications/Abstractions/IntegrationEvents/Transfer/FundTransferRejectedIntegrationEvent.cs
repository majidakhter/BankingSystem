using System;
using System.Collections.Generic;

namespace BankingAppDDD.Applications.Abstractions.IntegrationEvents.Transfer
{
    public sealed record FundTransferRejectedIntegrationEvent(
        Guid TransactionId,
        Guid AccountId,
        decimal Amount,
        string currencyCode,
        string RejectionReason,
        List<string> RiskFactors,
        DateTime RejectedAt,
        string CorrelationId) : MassTransitIntegrationEvent(CorrelationId);
}
