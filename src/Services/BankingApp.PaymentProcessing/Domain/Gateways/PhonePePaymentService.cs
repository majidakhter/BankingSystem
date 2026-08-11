using BankingAppDDD.Domains.Accounts.Models;
using BankingAppDDD.PaymentProcessing.Domain.Gateways.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;

namespace BankingAppDDD.PaymentProcessing.Domain.Gateways
{
    public class PhonePePaymentService : IPaymentGatewayService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PhonePePaymentService> _logger;
        private readonly AsyncPolicy _retryPolicy;

        public PhonePePaymentService(IConfiguration configuration, ILogger<PhonePePaymentService> logger)
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
                        _logger.LogWarning("PhonePe Payout network/gateway attempt {RetryCount} failed: {Message}. Retrying in {SleepDuration}s...",
                            retryCount, exception.Message, timeSpan.TotalSeconds);
                    });
        }

        public PaymentGatewayProvider Provider => PaymentGatewayProvider.PhonePe;

        public async Task<PayoutResponse> ProcessPayoutAsync(PayoutRequest request, CancellationToken cancellationToken = default)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                _logger.LogInformation("Processing PhonePe Payout for TransactionId: {TransactionId}, TransferType: {TransferType}, Amount: {Amount}",
                    request.TransactionId, request.TransferType, request.Amount);

                var merchantId = _configuration["PaymentGatewaySettings:PhonePe:MerchantId"] ?? "PGTESTPAYUAT";
                var mode = request.TransferType switch
                {
                    TransferType.NEFT => "NEFT",
                    TransferType.RTGS => "RTGS",
                    TransferType.UPI => "UPI",
                    _ => "UPI"
                };

                // Simulate network/API call with cancellation check
                await Task.Delay(200, cancellationToken);

                if (request.Amount <= 0)
                {
                    _logger.LogWarning("PhonePe Payout failed: Invalid amount {Amount}", request.Amount);
                    return new PayoutResponse(false, null, "PhonePe Payout error: Invalid transfer amount.");
                }

                var phonepeTxnId = $"T_PPE_{mode}_{Guid.NewGuid().ToString("N")[..12]}";
                _logger.LogInformation("PhonePe Payout successful for TransactionId: {TransactionId}, GatewayRef: {GatewayRef}",
                    request.TransactionId, phonepeTxnId);

                return new PayoutResponse(true, phonepeTxnId, null);
            });
        }
    }
}
