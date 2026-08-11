using BankingAppDDD.Domains.Accounts.Models;
using BankingAppDDD.PaymentProcessing.Domain.Gateways.Models;

namespace BankingAppDDD.PaymentProcessing.Domain.Gateways
{
    public interface IPaymentGatewayService
    {
        PaymentGatewayProvider Provider { get; }
        Task<PayoutResponse> ProcessPayoutAsync(PayoutRequest request, CancellationToken cancellationToken = default);
    }
}
