using Microsoft.ML.Data;

namespace BankingAppDDD.AccountManagement.Core.Accounts.Models
{
    public class TransactionData
    {
        [LoadColumn(0)] public float Amount { get; set; }
        [LoadColumn(1)] public float TransactionTime { get; set; } // Hour of day (0-23)
        [LoadColumn(2)] public float IsInternational { get; set; } // 0 = Domestic, 1 = International
        [LoadColumn(3)] public float DeviceRiskScore { get; set; } // Biometric/device risk (0.0 to 1.0)
        [LoadColumn(4)] public float HistoricalVelocity { get; set; } // Number of tx in last hour
        [LoadColumn(5)] public float PaymentType { get; set; } // 0 = BankTransfer (NEFT/RTGS/IMPS), 1 = UPI, 2 = Card

        [LoadColumn(6), ColumnName("Label")]
        public bool IsFraud { get; set; } // Target label for ML training
    }
}
