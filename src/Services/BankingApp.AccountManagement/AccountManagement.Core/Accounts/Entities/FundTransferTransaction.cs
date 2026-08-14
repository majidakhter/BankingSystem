using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Accounts.Models;

namespace BankingAppDDD.Domains.Accounts.Entities
{
    public sealed class FundTransferTransaction : EntityBase
    {
        private FundTransferTransaction()
        {
            Description = string.Empty;
        }

        public Guid TransactionId { get; private set; }
        public Guid AccountId { get; private set; }
        public Guid? DestinationAccountId { get; private set; }
        public decimal Amount { get; private set; }
        public string CurrencyCode { get; private set; }
        public TransferType TransferType { get; private set; }
        public TransferToEntity TransferToEntity { get; private set; }
        public PaymentGatewayProvider PaymentGateway { get; private set; }
        public TransferStatus Status { get; private set; }
        public string? BeneficiaryAccountNo { get; private set; }
        public string? IfscCode { get; private set; }
        public string? GatewayTransactionRef { get; private set; }
        public string? FailureReason { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public static FundTransferTransaction Create(
            Guid accountId,
            Guid? destinationAccountId,
            decimal amount,
            string currencyCode,
            TransferType transferType,
            TransferToEntity transferToEntity,
            PaymentGatewayProvider paymentGateway,
            string? beneficiaryAccountNo,
            string? ifscCode,
            string description,
            TransferStatus initialStatus = TransferStatus.Pending)
        {
            var transactionId = Guid.NewGuid();
            return new FundTransferTransaction
            {
                Id = transactionId,
                TransactionId = transactionId,
                AccountId = accountId,
                DestinationAccountId = destinationAccountId,
                Amount = amount,
                CurrencyCode = currencyCode,
                TransferType = transferType,
                TransferToEntity = transferToEntity,
                PaymentGateway = paymentGateway,
                Status = initialStatus,
                BeneficiaryAccountNo = beneficiaryAccountNo,
                IfscCode = ifscCode,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void MarkCompleted(string? gatewayTransactionRef)
        {
            Status = TransferStatus.Completed;
            GatewayTransactionRef = gatewayTransactionRef;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkFailed(string reason)
        {
            Status = TransferStatus.Failed;
            FailureReason = reason;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkReversed(string reason)
        {
            Status = TransferStatus.Reversed;
            FailureReason = reason;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

