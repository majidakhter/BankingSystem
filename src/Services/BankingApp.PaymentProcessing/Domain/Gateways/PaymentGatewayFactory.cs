using BankingAppDDD.Domains.Accounts.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BankingAppDDD.PaymentProcessing.Domain.Gateways
{
    public interface IPaymentGatewayFactory
    {
        IPaymentGatewayService GetGatewayService(PaymentGatewayProvider provider);
    }

    public class PaymentGatewayFactory : IPaymentGatewayFactory
    {
        private readonly IEnumerable<IPaymentGatewayService> _gatewayServices;

        public PaymentGatewayFactory(IEnumerable<IPaymentGatewayService> gatewayServices)
        {
            _gatewayServices = gatewayServices;
        }

        public IPaymentGatewayService GetGatewayService(PaymentGatewayProvider provider)
        {
            var service = _gatewayServices.FirstOrDefault(s => s.Provider == provider);
            if (service == null)
            {
                // Fallback to RazorPay if not explicitly matched
                service = _gatewayServices.FirstOrDefault(s => s.Provider == PaymentGatewayProvider.RazorPay)
                    ?? throw new NotSupportedException($"Payment gateway provider '{provider}' is not supported.");
            }
            return service;
        }
    }
}
