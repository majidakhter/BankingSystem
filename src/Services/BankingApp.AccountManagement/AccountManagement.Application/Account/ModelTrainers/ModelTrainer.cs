using BankingAppDDD.AccountManagement.Core.Accounts.Models;
using Microsoft.Extensions.Logging;
using Microsoft.ML;

namespace BankingAppDDD.AccountManagement.Infrastructure.Repositories.Implementation
{
    public class ModelTrainer
    {
        private readonly ILogger<ModelTrainer>? _logger;

        public ModelTrainer(ILogger<ModelTrainer>? logger = null)
        {
            _logger = logger;
        }

        public void TrainAndSaveModel(string modelPath = "Models/fraud_model.zip")
        {
            try
            {
                _logger?.LogInformation("Starting ML.NET Fraud Detection Model training...");

                MLContext mlContext = new MLContext(seed: 42);

                // Create comprehensive synthetic training dataset covering Bank Transfers, UPI, and Cards
                var trainingData = GenerateSyntheticDataset();

                IDataView dataView = mlContext.Data.LoadFromEnumerable(trainingData);

                // Define feature pipeline mapping all 6 features
                var pipeline = mlContext.Transforms.Concatenate(
                        "Features",
                        nameof(TransactionData.Amount),
                        nameof(TransactionData.TransactionTime),
                        nameof(TransactionData.IsInternational),
                        nameof(TransactionData.DeviceRiskScore),
                        nameof(TransactionData.HistoricalVelocity),
                        nameof(TransactionData.PaymentType))
                    .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                        labelColumnName: "Label",
                        featureColumnName: "Features"));

                _logger?.LogInformation("Fitting ML.NET model on training dataset...");
                var model = pipeline.Fit(dataView);

                string? dir = Path.GetDirectoryName(modelPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                mlContext.Model.Save(model, dataView.Schema, modelPath);
                _logger?.LogInformation("Successfully trained and saved Fraud Detection ML model to {ModelPath}", modelPath);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to train and save ML Fraud Detection Model.");
                throw;
            }
        }

        private List<TransactionData> GenerateSyntheticDataset()
        {
            var data = new List<TransactionData>();

            // Legitimate transactions (Low amount, normal hours 8-22, domestic, low device risk)
            for (int i = 0; i < 300; i++)
            {
                data.Add(new TransactionData
                {
                    Amount = (float)(10 + (i % 200)),
                    TransactionTime = 8 + (i % 14),
                    IsInternational = 0f,
                    DeviceRiskScore = 0.05f + (float)(i % 10) * 0.01f,
                    HistoricalVelocity = 1f + (i % 3),
                    PaymentType = i % 3, // 0 = BankTransfer, 1 = UPI, 2 = Card
                    IsFraud = false
                });
            }

            // High Risk / Fraudulent transactions (High amount > 5000, odd hours 1-4 AM, international, high device risk, high velocity)
            for (int i = 0; i < 100; i++)
            {
                data.Add(new TransactionData
                {
                    Amount = 6000f + (i * 150),
                    TransactionTime = 1 + (i % 4),
                    IsInternational = (i % 2 == 0) ? 1f : 0f,
                    DeviceRiskScore = 0.75f + (float)(i % 25) * 0.01f,
                    HistoricalVelocity = 8f + (i % 5),
                    PaymentType = i % 3,
                    IsFraud = true
                });
            }

            // Medium Risk transactions (Moderate amount 1500-4000, evening hours, moderate device risk)
            for (int i = 0; i < 100; i++)
            {
                data.Add(new TransactionData
                {
                    Amount = 1500f + (i * 30),
                    TransactionTime = 22 + (i % 2),
                    IsInternational = 0f,
                    DeviceRiskScore = 0.40f + (float)(i % 20) * 0.01f,
                    HistoricalVelocity = 4f + (i % 3),
                    PaymentType = i % 3,
                    IsFraud = false
                });
            }

            return data;
        }
    }
}
