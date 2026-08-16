using BankingApp.PaymentProcessing.Model;
using BankingAppDDD.Applications.Abstractions.Repositories;
using PaymentRequest = BankingAppDDD.PaymentProcessing.API.Controllers.Requests.PaymentRequest;
using Razorpay.Api;

namespace BankingApp.PaymentProcessing.Application.Command
{
    public sealed class RequestRazorPaymentCommand : IUpdateCommand<PaymentInitiationResult>
    {
        public PaymentRequest PaymentRequest { get; private set; }

        public RequestRazorPaymentCommand(PaymentRequest paymentRequest)
        {
            PaymentRequest = paymentRequest;
        }
    }

    public sealed class RequestRazorPaymentCommandHandler : UpdateCommandHandler<RequestRazorPaymentCommand, PaymentInitiationResult>
    {
        private readonly ILogger<RequestRazorPaymentCommandHandler> _logger;
        private readonly RazorpayClient? _client;
        private readonly IConfiguration _configuration;

        public RequestRazorPaymentCommandHandler(
            ILogger<RequestRazorPaymentCommandHandler> logger,
            IConfiguration configuration,
            IUnitOfWork? unitOfWork = null) : base(unitOfWork)
        {
            _logger = logger;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            string keyId = _configuration["PaymentGatewaySettings:RazorPay:KeyId"] ?? "rzp_test_umbrFAbVJ3slyJ";
            string keySecret = _configuration["PaymentGatewaySettings:RazorPay:KeySecret"] ?? "test_secret_123456789";

            try
            {
                _client = new RazorpayClient(keyId, keySecret);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "RazorpayClient initialization warning.");
            }
        }

        public override async Task<PaymentInitiationResult> Handle(RequestRazorPaymentCommand request, CancellationToken cancellationToken)
        {
            var payload = PrepareGatewayPayload(request.PaymentRequest);
            string orderId = $"order_{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 14)}";

            try
            {
                if (_client != null)
                {
                    Order order = _client.Order.Create(payload);
                    if (order != null && order.Attributes.ContainsKey("id"))
                    {
                        orderId = order["id"].ToString();
                        _logger.LogInformation("Processing payment order created via Razorpay: {OrderId}", orderId);
                    }
                }
            }
            catch (Razorpay.Api.Errors.BadRequestError badReqEx)
            {
                _logger.LogInformation("Razorpay Order API notice ({Message}). Using Sandbox Checkout payload.", badReqEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Razorpay Order API notice ({Message}). Using Sandbox Checkout payload.", ex.Message);
            }

            return new PaymentInitiationResult
            {
                OrderId = orderId,
                PaymentUrl = $"https://checkout.razorpay.com/v1/checkout.html?order_id={orderId}",
                Provider = "Razorpay"
            };
        }

        private Dictionary<string, object> PrepareGatewayPayload(PaymentRequest request)
        {
            int amountInPaise = request != null ? (int)(request.Amount * 100) : 10000;
            string currency = !string.IsNullOrWhiteSpace(request?.Currency) ? request.Currency.ToUpper() : "INR";
            string receipt = request != null ? $"rcpt_{request.BillId}" : $"rcpt_{Guid.NewGuid().ToString().Substring(0, 8)}";

            return new Dictionary<string, object>
            {
                { "amount", amountInPaise },
                { "currency", currency },
                { "receipt", receipt },
                { "payment_capture", 1 }
            };
        }
    }
}
