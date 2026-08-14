using BankingAppDDD.Applications.Abstractions.IntegrationEvents.Transfer;
using BankingAppDDD.PaymentProcessing.Domain.Gateways;
using BankingAppDDD.PaymentProcessing.Domain.Gateways.Models;
using MassTransit;

namespace BankingAppDDD.PaymentProcessing.Application.ProcessingPayment.Consumers
{
    public sealed class FundTransferRequestedConsumer : IConsumer<FundTransferRequestedIntegrationEvent>
    {
        private readonly IPaymentGatewayFactory _gatewayFactory;
        private readonly ILogger<FundTransferRequestedConsumer> _logger;

        public FundTransferRequestedConsumer(
            IPaymentGatewayFactory gatewayFactory,
            ILogger<FundTransferRequestedConsumer> logger)
        {
            _gatewayFactory = gatewayFactory;
            _logger = logger; 
        }

        public async Task Consume(ConsumeContext<FundTransferRequestedIntegrationEvent> context)
        {
            var @event = context.Message;
            _logger.LogInformation("Processing FundTransferRequestedIntegrationEvent for TransactionId: {TransactionId}, TransferType: {TransferType}, Gateway: {Gateway}",
                @event.TransactionId, @event.TransferType, @event.transferToEntity);

            try
            {
                var gateway = _gatewayFactory.GetGatewayService(@event.PaymentGateway);
                var payoutRequest = new PayoutRequest(
                    @event.TransactionId,
                    @event.AccountId,
                    @event.Amount,
                    @event.TransferType,
                    @event.PaymentGateway,
                    @event.BeneficiaryAccountNo,
                    @event.DestinationBankIfscCode,
                    @event.Description);

                var response = await gateway.ProcessPayoutAsync(payoutRequest, context.CancellationToken);

                if (response.IsSuccess)
                {
                    _logger.LogInformation("Payout succeeded via gateway {Gateway} for TransactionId: {TransactionId}, GatewayRef: {GatewayRef}",
                        @event.PaymentGateway, @event.TransactionId, response.GatewayTransactionRef);

                    var completedEvent = new FundTransferCompletedIntegrationEvent(
                        @event.TransactionId,
                        @event.AccountId,
                        response.GatewayTransactionRef,
                        @event.CorrelationId);

                    await context.Publish(completedEvent);
                }
                else
                {
                    _logger.LogWarning("Payout failed via gateway {Gateway} for TransactionId: {TransactionId}, Error: {Error}",
                        @event.PaymentGateway, @event.TransactionId, response.ErrorMessage);

                    var failedEvent = new FundTransferFailedIntegrationEvent(
                        @event.TransactionId,
                        @event.AccountId,
                        @event.Amount,
                        @event.currencyCode,
                        response.ErrorMessage ?? "Payment Gateway payout failed",
                        @event.CorrelationId);

                    await context.Publish(failedEvent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while processing payout for TransactionId: {TransactionId}", @event.TransactionId);

                var failedEvent = new FundTransferFailedIntegrationEvent(
                    @event.TransactionId,
                    @event.AccountId,
                    @event.Amount,
                    @event.currencyCode,
                    ex.Message,
                    @event.CorrelationId);

                await context.Publish(failedEvent);
            }
        }
    }
}
