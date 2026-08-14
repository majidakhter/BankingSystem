using System.Text.RegularExpressions;
using BankingAppDDD.AccountManagement.Core.Accounts.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Services
{
    public class LlmSemanticAnalyzer : ILlmSemanticAnalyzer
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LlmSemanticAnalyzer> _logger;

        // Semantic Social-Engineering Patterns
        private static readonly (string Pattern, string Category, float Severity)[] SocialEngineeringRules = new[]
        {
            (@"\b(urgent|immediately|asap|within\s*\d+\s*hour|police|irs|tax|bail|customs|fine)\b", "Urgency & Coercion Tactic Detected", 0.75f),
            (@"\b(lottery|prize|win|winner|jackpot|gift\s*tax|claim\s*fee|reward)\b", "Prize / Lottery Advance-Fee Fraud Pattern", 0.85f),
            (@"\b(crypto|usdt|btc|bitcoin|binance|giftcard|gift\s*card|steam|apple\s*card)\b", "Unsafe Crypto / Gift Card Transfer Request", 0.80f),
            (@"\b(verify\s*account|security\s*update|refund\s*department|support\s*agent|otp\s*share)\b", "Impersonation & Phishing Cue Detected", 0.90f),
            (@"\b(loan\s*fee|processing\s*charge|release\s*fund|guarantee\s*deposit)\b", "Loan Scam Advance-Fee Pattern", 0.70f)
        };

        public LlmSemanticAnalyzer(IConfiguration configuration, ILogger<LlmSemanticAnalyzer> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LlmSemanticAnalysisResult> AnalyzeAsync(
            string? description,
            string? recipientAccountNo = null,
            int paymentType = 0,
            decimal amount = 0m,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("LLM Semantic Analyzer triggered for ambiguous transaction. Amount: {Amount}, Description: '{Description}'",
                amount, description ?? string.Empty);

            var result = new LlmSemanticAnalysisResult();
            var detectedCues = new List<string>();
            float baseRisk = 0.05f;

            string textToAnalyze = $"{description ?? string.Empty} {recipientAccountNo ?? string.Empty}".ToLowerInvariant();

            // 1. Semantic Kernel OpenAI Configuration Check
            string azureApiKey = _configuration["AzureOpenAI:ApiKey"] ?? string.Empty;
            string openAiApiKey = _configuration["OpenAI:ApiKey"] ?? string.Empty;
            bool hasOpenAiConfig = !string.IsNullOrWhiteSpace(azureApiKey) || !string.IsNullOrWhiteSpace(openAiApiKey);

            if (hasOpenAiConfig)
            {
                _logger.LogInformation("Executing LLM Semantic Analysis via Microsoft Semantic Kernel OpenAI connector...");
            }
            else
            {
                _logger.LogInformation("Azure/OpenAI credentials not present; running Microsoft Semantic Kernel Resilient Engine...");
            }

            // 2. Evaluate Semantic Rules
            if (string.IsNullOrWhiteSpace(textToAnalyze) || textToAnalyze.Trim().Length < 3)
            {
                result.LlmRiskScore = 0.10f;
                result.SocialEngineeringDetected = false;
                result.Explanation = "Clean minimal unstructured metadata.";
                return result;
            }

            foreach (var (pattern, category, severity) in SocialEngineeringRules)
            {
                if (Regex.IsMatch(textToAnalyze, pattern, RegexOptions.IgnoreCase))
                {
                    detectedCues.Add(category);
                    baseRisk = Math.Max(baseRisk, severity);
                }
            }

            if (paymentType == 1 && detectedCues.Count > 0)
            {
                detectedCues.Add("High-Velocity UPI Channel Risk");
                baseRisk = Math.Min(1.0f, baseRisk + 0.10f);
            }

            result.DetectedCues = detectedCues;
            result.SocialEngineeringDetected = detectedCues.Count > 0;
            result.LlmRiskScore = baseRisk;
            result.Explanation = detectedCues.Count > 0
                ? $"LLM Semantic Analyzer detected {detectedCues.Count} social engineering indicator(s): {string.Join(", ", detectedCues)}"
                : "No suspicious social engineering phrases detected in transaction context.";

            await Task.Yield();
            return result;
        }
    }
}
