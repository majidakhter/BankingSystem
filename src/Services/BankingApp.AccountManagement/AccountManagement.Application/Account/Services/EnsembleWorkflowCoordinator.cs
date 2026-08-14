using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BankingAppDDD.AccountManagement.Core.Accounts.Models;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Services
{
    public class EnsembleWorkflowCoordinator : IEnsembleWorkflowCoordinator
    {
        private readonly ILlmSemanticAnalyzer _llmSemanticAnalyzer;
        private readonly ILogger<EnsembleWorkflowCoordinator> _logger;

        public EnsembleWorkflowCoordinator(
            ILlmSemanticAnalyzer llmSemanticAnalyzer,
            ILogger<EnsembleWorkflowCoordinator> logger)
        {
            _llmSemanticAnalyzer = llmSemanticAnalyzer;
            _logger = logger;
        }

        public async Task<EnsembleDecisionResult> EvaluateEnsembleAsync(
            float mlRiskScore,
            bool mlIsFraud,
            string? description,
            string? recipientAccountNo = null,
            int paymentType = 0,
            decimal amount = 0m,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Ensemble Workflow Coordinator evaluating ML Score: {MlScore:F2}, Amount: {Amount}", mlRiskScore, amount);

            var result = new EnsembleDecisionResult
            {
                MlRiskScore = mlRiskScore
            };

            var riskFactors = new List<string>();

            if (mlRiskScore > 0.40f)
            {
                riskFactors.Add($"ML Quantitative Risk Score Elevated: {mlRiskScore * 100:F0}%");
            }

            bool inAmbiguousRange = (mlRiskScore >= 0.25f && mlRiskScore <= 0.70f) || amount > 3000m || !string.IsNullOrWhiteSpace(description);

            if (inAmbiguousRange)
            {
                result.LlmAnalyzerTriggered = true;
                _logger.LogInformation("ML score in ambiguous range ({MlScore:F2}). Triggering LLM Semantic Analyzer...", mlRiskScore);

                var llmAnalysis = await _llmSemanticAnalyzer.AnalyzeAsync(
                    description,
                    recipientAccountNo,
                    paymentType,
                    amount,
                    cancellationToken);

                result.LlmRiskScore = llmAnalysis.LlmRiskScore;

                if (llmAnalysis.DetectedCues != null && llmAnalysis.DetectedCues.Count > 0)
                {
                    riskFactors.AddRange(llmAnalysis.DetectedCues);
                }

                float weightedScore = (0.60f * mlRiskScore) + (0.40f * llmAnalysis.LlmRiskScore);

                if (llmAnalysis.SocialEngineeringDetected && llmAnalysis.LlmRiskScore >= 0.75f)
                {
                    _logger.LogWarning("Severe social-engineering cues detected by LLM Semantic Analyzer. Escalating ensemble risk score.");
                    weightedScore = Math.Max(weightedScore, llmAnalysis.LlmRiskScore);
                }

                result.FinalEnsembleScore = Math.Clamp(weightedScore, 0.0f, 1.0f);
                result.DecisionReason = $"Ensemble Decision: ML ({mlRiskScore:F2}) + LLM Semantic ({llmAnalysis.LlmRiskScore:F2}) -> Merged Ensemble Score ({result.FinalEnsembleScore:F2}). {llmAnalysis.Explanation}";
            }
            else
            {
                result.LlmAnalyzerTriggered = false;
                result.FinalEnsembleScore = mlRiskScore;
                result.DecisionReason = $"Clear quantitative assessment from Layer 1 ML Engine (Score: {mlRiskScore:F2}).";
            }

            if (mlIsFraud)
            {
                result.FinalEnsembleScore = Math.Max(result.FinalEnsembleScore, 0.85f);
                riskFactors.Add("ML Classification flagged high-probability fraud.");
            }

            if (result.FinalEnsembleScore > 0.70f)
            {
                result.Action = "BLOCK_TRANSACTION";
            }
            else if (result.FinalEnsembleScore > 0.35f)
            {
                result.Action = "TRIGGER_MFA_STEP_UP";
            }
            else
            {
                result.Action = "ALLOW";
            }

            result.MergedRiskFactors = riskFactors;
            _logger.LogInformation("Ensemble Workflow final decision: Action={Action}, EnsembleScore={Score:F2}", result.Action, result.FinalEnsembleScore);

            return result;
        }
    }
}
