using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.AccountManagement.Application.Outbox;
using BankingAppDDD.AccountManagement.Core.Accounts.Models;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.IntegrationEvents.Transfer;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.Domains.Accounts.Models;
using BankingAppDDD.MongoService.Application.Mongo;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using System;
using System.Threading.Tasks;

namespace BankingApp.AccountManagement.Application.Transfers.Commands
{
    public sealed record TransferFundsCommand(
        Guid senderAccountId,
        string senderBankIfscCode,
        Guid receiverAccountId,
        string receiverBankIfscCode,
        decimal amount,
        string currencyCode,
        string description,
        TransferType transferType, // NEFT, RTGS, IMPS
        TransferToEntity transferToEntity, // OwnBankAccount, OtherBank
        DateTime? transactiontime = null,
        string? receiverAccountNo = null,
        string? receivermobileNo = null, // Required when doing IMPS transaction (account to mobile)
        PaymentGatewayProvider paymentGateway = PaymentGatewayProvider.Internal,
        string? otpCode = null) : Command;

    public sealed class TransferFundsCommandHandler : CommandHandler<TransferFundsCommand>
    {
        private readonly IAccountRepository<Account> _repository;
        private readonly IRepository<BeneficiaryGroup> _beneficiaryRepository;
        private readonly IRepository<FundTransferTransaction> _transferRepository;
        private readonly IOutboxService _outboxService;
        private readonly IAccountMongoService? _mongoService;
        private readonly PredictionEnginePool<TransactionData, FraudPrediction>? _predictionEnginePool;
        private readonly ILogger<TransferFundsCommandHandler> logger;

        public TransferFundsCommandHandler(
            IAccountRepository<Account> repository,
            IRepository<BeneficiaryGroup> beneficiaryRepository,
            IRepository<FundTransferTransaction> transferRepository,
            IOutboxService outboxService,
            ILogger<TransferFundsCommandHandler> _logger,
            IUnitOfWork unitOfWork,
            IAccountMongoService? mongoService = null,
            PredictionEnginePool<TransactionData, FraudPrediction>? predictionEnginePool = null) : base(unitOfWork)
        {
            _repository = repository;
            _beneficiaryRepository = beneficiaryRepository;
            _transferRepository = transferRepository;
            _outboxService = outboxService;
            logger = _logger;
            _mongoService = mongoService;
            _predictionEnginePool = predictionEnginePool;
        }

        protected override async Task<bool> HandleAsync(TransferFundsCommand request)
        {
            var correlationId = Guid.NewGuid().ToString();
            logger.LogInformation("Decoupled Intake Gateway receiving fund transfer request. CorrelationId: {CorrelationId}, AccountId: {AccountId}, TransferType: {TransferType}, Amount: {Amount}",
                correlationId, request.senderAccountId, request.transferType, request.amount);

            // 1. Fast-Path Inline Validation & Hard-Stops (Sub-millisecond)
            if (request.amount <= 0m)
            {
                throw new ArgumentException("Transfer amount must be greater than zero.");
            }

            var originAccount = await _repository.GetEntityById(request.senderAccountId);
            if (originAccount is not Account withdrawAccount)
            {
                throw new Exception($"Source Account {request.senderAccountId} not found.");
            }

            var currentBal = originAccount.GetCurrentBalance().Value;
            if (request.amount > currentBal)
            {
                throw new InvalidOperationException($"Insufficient account balance. Available: ${currentBal:F2}, Requested: ${request.amount:F2}");
            }

            // Fast-Path inline ML hard-stop check (velocity / blacklist check)
            if (_predictionEnginePool != null)
            {
                var fastTxData = new TransactionData
                {
                    Amount = (float)request.amount,
                    TransactionTime = DateTime.UtcNow.Hour,
                    IsInternational = 0f,
                    DeviceRiskScore = 0.05f,
                    HistoricalVelocity = 1f,
                    PaymentType = 0f,
                    IsFraud = false
                };

                try
                {
                    FraudPrediction fastPrediction;
                    try
                    {
                        fastPrediction = _predictionEnginePool.Predict(fastTxData);
                    }
                    catch (ArgumentException)
                    {
                        fastPrediction = _predictionEnginePool.Predict("FraudDetectionModel", fastTxData);
                    }

                    if (fastPrediction.IsFraudulent && request.amount > 25000m)
                    {
                        logger.LogWarning("Fast-Path inline hard-stop triggered! High-velocity fraud pattern detected.");
                        throw new InvalidOperationException("Fast-Path Hard-Stop: Transaction blocked by inline velocity check.");
                    }
                }
                catch (InvalidOperationException) { throw; }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Fast-Path inline check bypassed; proceeding to async evaluation pipeline.");
                }
            }

            // 2. Resolve Receiver Identification based on TransferType
            string? effectiveReceiverAccountNo = request.receiverAccountNo;
            Guid effectiveReceiverAccountId = request.receiverAccountId;

            if (request.transferType == TransferType.IMPS)
            {
                if (string.IsNullOrWhiteSpace(request.receivermobileNo))
                {
                    throw new ArgumentException("Receiver mobile number (receivermobileNo) is required for IMPS transfers.");
                }

                if (string.IsNullOrWhiteSpace(effectiveReceiverAccountNo))
                {
                    var beneficiary = await _beneficiaryRepository.FirstOrDefaultAsync(b => b.Beneficiary != null && b.Beneficiary.BeneficaryAccountNo.ToString() == request.receivermobileNo);
                    if (beneficiary != null)
                    {
                        effectiveReceiverAccountNo = beneficiary.Beneficiary.BeneficaryAccountNo.ToString();
                    }
                    else
                    {
                        effectiveReceiverAccountNo = request.receivermobileNo;
                    }
                }
            }
            else // NEFT & RTGS
            {
                if (string.IsNullOrWhiteSpace(effectiveReceiverAccountNo) && effectiveReceiverAccountId == Guid.Empty)
                {
                    throw new ArgumentException($"Receiver account ID or account number is required for {request.transferType} transfers.");
                }
            }

            // 3. State Machine Lifecycle Initialization: Create Transaction in PendingVerification state
            var pendingTransaction = FundTransferTransaction.Create(
                request.senderAccountId,
                effectiveReceiverAccountId == Guid.Empty ? null : effectiveReceiverAccountId,
                request.amount,
                request.currencyCode,
                request.transferType,
                request.transferToEntity,
                request.paymentGateway,
                effectiveReceiverAccountNo,
                request.receiverBankIfscCode,
                request.description ?? $"Pending Verification via {request.transferType}",
                TransferStatus.PendingVerification);

            _transferRepository.Insert(pendingTransaction);

            // 4. Publish FundTransferSubmittedIntegrationEvent to Outbox in local DB transaction
            var submittedEvent = new FundTransferSubmittedIntegrationEvent(
                pendingTransaction.TransactionId,
                request.senderAccountId,
                withdrawAccount.AccountNo,
                request.senderBankIfscCode,
                effectiveReceiverAccountId,
                request.amount,
                request.currencyCode,
                request.description ?? "Fund transfer submitted",
                request.transferType,
                request.transferToEntity,
                request.paymentGateway,
                effectiveReceiverAccountNo,
                request.receiverBankIfscCode,
                correlationId);

            await _outboxService.SaveEventAsync("FundTransferTransaction", pendingTransaction.TransactionId.ToString(), submittedEvent);

            // 5. Commit local DB transaction ATOMICALLY (Record in PendingVerification state + Outbox Event)
            await this.UnitOfWork.CommitAsync();

            if (_mongoService != null)
            {
                await _mongoService.SaveTransferTransactionAsync(pendingTransaction);
            }

            logger.LogInformation("Intake Gateway accepted transaction {TransactionId} (Status: PENDING_VERIFICATION). FundTransferSubmittedIntegrationEvent published to RabbitMQ via Outbox.",
                pendingTransaction.TransactionId);

            return true;
        }
    }
}
