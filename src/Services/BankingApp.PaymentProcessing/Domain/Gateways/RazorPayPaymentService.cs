using BankingAppDDD.Domains.Accounts.Models;
using BankingAppDDD.PaymentProcessing.Domain.Gateways.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;

namespace BankingAppDDD.PaymentProcessing.Domain.Gateways
{
    public class RazorPayPaymentService : IPaymentGatewayService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RazorPayPaymentService> _logger;
        private readonly AsyncPolicy _retryPolicy;

        public RazorPayPaymentService(IConfiguration configuration, ILogger<RazorPayPaymentService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Configure Polly retry policy for transient gateway/network failures (3 retries with exponential backoff)
            _retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning("RazorPay Payout network/gateway attempt {RetryCount} failed: {Message}. Retrying in {SleepDuration}s...",
                            retryCount, exception.Message, timeSpan.TotalSeconds);
                    });
        }

        public PaymentGatewayProvider Provider => PaymentGatewayProvider.RazorPay;

        public async Task<PayoutResponse> ProcessPayoutAsync(PayoutRequest request, CancellationToken cancellationToken = default)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                _logger.LogInformation("Processing RazorPay Payout for TransactionId: {TransactionId}, TransferType: {TransferType}, Amount: {Amount}",
                    request.TransactionId, request.TransferType, request.Amount);

                var keyId = _configuration["PaymentGatewaySettings:RazorPay:KeyId"] ?? "rzp_test_default";
                var mode = request.TransferType switch
                {
                    TransferType.NEFT => "NEFT",
                    TransferType.RTGS => "RTGS",
                    TransferType.UPI => "UPI",
                    _ => "NEFT"
                };

                // Simulate network/API call with cancellation check
                await Task.Delay(200, cancellationToken);

                if (request.Amount <= 0)
                {
                    _logger.LogWarning("RazorPay Payout failed: Invalid amount {Amount}", request.Amount);
                    return new PayoutResponse(false, null, "RazorPay Payout error: Invalid transfer amount.");
                }

                var razorpayPayoutId = $"pout_rzp_{mode}_{Guid.NewGuid().ToString("N")[..12]}";
                _logger.LogInformation("RazorPay Payout successful for TransactionId: {TransactionId}, GatewayRef: {GatewayRef}",
                    request.TransactionId, razorpayPayoutId);

                return new PayoutResponse(true, razorpayPayoutId, null);
            });
        }
    }
}
