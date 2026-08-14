using System;
using System.Collections.Generic;

namespace BankingAppDDD.AccountManagement.Core.Accounts.Models
{
    public class FraudEvaluationResult
    {
        public float TransactionAmount { get; set; }
        public bool IsFraudulent { get; set; }
        public float RiskScore { get; set; }
        public float MlRiskScore { get; set; }
        public float LlmRiskScore { get; set; }
        public float EnsembleScore { get; set; }
        public bool LlmAnalyzerTriggered { get; set; }
        public string Action { get; set; } = "ALLOW"; // "ALLOW", "TRIGGER_MFA_STEP_UP", "BLOCK_TRANSACTION"
        public string Message { get; set; } = string.Empty;
        public int PaymentType { get; set; } // 0 = BankTransfer, 1 = UPI, 2 = Card
        public List<string> RiskFactors { get; set; } = new();
        public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
    }
}
