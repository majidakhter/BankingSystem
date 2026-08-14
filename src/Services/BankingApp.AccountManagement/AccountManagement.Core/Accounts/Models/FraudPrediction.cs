using Microsoft.ML.Data;

namespace BankingAppDDD.AccountManagement.Core.Accounts.Models
{
    public class FraudPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool IsFraudulent { get; set; }

        [ColumnName("Score")]
        public float RiskScore { get; set; } // Probability metrics output

        public float Probability { get; set; }
    }
}
