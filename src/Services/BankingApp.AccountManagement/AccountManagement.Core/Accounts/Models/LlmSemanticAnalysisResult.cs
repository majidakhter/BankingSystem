using System;
using System.Collections.Generic;

namespace BankingAppDDD.AccountManagement.Core.Accounts.Models
{
    public class LlmSemanticAnalysisResult
    {
        public float LlmRiskScore { get; set; }
        public bool SocialEngineeringDetected { get; set; }
        public List<string> DetectedCues { get; set; } = new();
        public string Explanation { get; set; } = string.Empty;
        public float ConfidenceScore { get; set; } = 1.0f;
    }
}
