using System;
using System.Collections.Generic;

namespace BankingAppDDD.AccountManagement.Core.Accounts.Models
{
    public class EnsembleDecisionResult
    {
        public float FinalEnsembleScore { get; set; }
        public float MlRiskScore { get; set; }
        public float LlmRiskScore { get; set; }
        public bool LlmAnalyzerTriggered { get; set; }
        public string Action { get; set; } = "ALLOW"; // "ALLOW", "TRIGGER_MFA_STEP_UP", "BLOCK_TRANSACTION"
        public List<string> MergedRiskFactors { get; set; } = new();
        public string DecisionReason { get; set; } = string.Empty;
    }
}
