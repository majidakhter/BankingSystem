using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApp.AccountManagement.Application.Services;
using BankingAppDDD.AccountManagement.Application.Outbox;
using BankingAppDDD.AccountManagement.Core.Accounts.Models;
using BankingAppDDD.Applications.Abstractions.IntegrationEvents.Transfer;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Accounts.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;

namespace BankingApp.AccountManagement.Application.Transfer.Consumers
{
    /// <summary>
    /// Async Risk Scoring Consumer consuming FundTransferSubmittedIntegrationEvent from RabbitMQ.
    /// Executes Layer 1 ML + Layer 2 Microsoft Semantic Kernel LLM Ensemble Risk Evaluation off the HTTP thread,
    /// and emits FraudEvaluationCompletedIntegrationEvent.
    /// </summary>
    public sealed class AsyncFraudEvaluationConsumer : IConsumer<FundTransferSubmittedIntegrationEvent>
    {
        private readonly PredictionEnginePool<TransactionData, FraudPrediction>? _predictionEnginePool;
        private readonly IEnsembleWorkflowCoordinator? _ensembleCoordinator;
        private readonly ILogger<AsyncFraudEvaluationConsumer> _logger;

        public AsyncFraudEvaluationConsumer(
            ILogger<AsyncFraudEvaluationConsumer> logger,
            PredictionEnginePool<TransactionData, FraudPrediction>? predictionEnginePool = null,
            IEnsembleWorkflowCoordinator? ensembleCoordinator = null)
        {
            _logger = logger;
            _predictionEnginePool = predictionEnginePool;
            _ensembleCoordinator = ensembleCoordinator;
        }

        public async Task Consume(ConsumeContext<FundTransferSubmittedIntegrationEvent> context)
        {
            var @event = context.Message;
            _logger.LogInformation("AsyncFraudEvaluationConsumer evaluating FundTransferSubmittedIntegrationEvent for TransactionId: {TransactionId}, Amount: {Amount}",
                @event.TransactionId, @event.Amount);

            float mlScore = 0.10f;
            bool mlIsFraud = false;

            if (_predictionEnginePool != null)
            {
                var txData = new TransactionData
                {
                    Amount = (float)@event.Amount,
                    TransactionTime = DateTime.UtcNow.Hour,
                    IsInternational = 0f,
                    DeviceRiskScore = 0.05f,
                    HistoricalVelocity = 1f,
                    PaymentType = 0f,
                    IsFraud = false
                };

                try
                {
                    FraudPrediction prediction;
                    try
                    {
                        prediction = _predictionEnginePool.Predict(txData);
                    }
                    catch (ArgumentException)
                    {
                        prediction = _predictionEnginePool.Predict("FraudDetectionModel", txData);
                    }
                    mlScore = prediction.Probability > 0 ? prediction.Probability : Math.Clamp(prediction.RiskScore, 0.0f, 1.0f);
                    mlIsFraud = prediction.IsFraudulent;

                    if (@event.Amount > 8000m) mlScore = Math.Max(mlScore, 0.75f);
                    else if (@event.Amount > 3000m) mlScore = Math.Max(mlScore, 0.45f);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Async ML evaluation failed; proceeding with fallback score.");
                }
            }

            var ensembleResult = new EnsembleDecisionResult
            {
                Action = "ALLOW",
                FinalEnsembleScore = mlScore,
                MlRiskScore = mlScore,
                LlmRiskScore = 0.05f,
                MergedRiskFactors = new List<string>()
            };

            if (_ensembleCoordinator != null)
            {
                ensembleResult = await _ensembleCoordinator.EvaluateEnsembleAsync(
                    mlScore,
                    mlIsFraud,
                    @event.Description,
                    @event.BeneficiaryAccountNo,
                    (int)@event.TransferType,
                    @event.Amount);
            }

            _logger.LogInformation("Async Fraud Evaluation completed for Transaction {TransactionId}: Action={Action}, EnsembleScore={Score:F2}",
                @event.TransactionId, ensembleResult.Action, ensembleResult.FinalEnsembleScore);

            // Emit FraudEvaluationCompletedIntegrationEvent to trigger Two-Phase State Processor
            var completedEvent = new FraudEvaluationCompletedIntegrationEvent(
                @event.TransactionId,
                @event.AccountId,
                @event.AccountNo,
                @event.senderBankIfscCode,
                @event.DestinationAccountId,
                @event.Amount,
                @event.currencyCode,
                @event.Description,
                @event.TransferType,
                @event.transferToEntity,
                @event.PaymentGateway,
                @event.BeneficiaryAccountNo,
                @event.receiverBankIfscCode,
                ensembleResult.Action,
                ensembleResult.MlRiskScore,
                ensembleResult.LlmRiskScore,
                ensembleResult.FinalEnsembleScore,
                ensembleResult.MergedRiskFactors ?? new List<string>(),
                @event.CorrelationId);

            await context.Publish(completedEvent);
            _logger.LogInformation("Published FraudEvaluationCompletedIntegrationEvent for TransactionId: {TransactionId}", @event.TransactionId);
        }
    }
}
