using System.Threading;
using System.Threading.Tasks;
using BankingAppDDD.AccountManagement.Core.Accounts.Models;

namespace BankingApp.AccountManagement.Application.Services
{
    public interface IEnsembleWorkflowCoordinator
    {
        Task<EnsembleDecisionResult> EvaluateEnsembleAsync(
            float mlRiskScore,
            bool mlIsFraud,
            string? description,
            string? recipientAccountNo = null,
            int paymentType = 0,
            decimal amount = 0m,
            CancellationToken cancellationToken = default);
    }
}
