using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.Guards;
using BankingAppDDD.Domains.Accounts.Models;
using BankingAppDDD.PaymentProcessing.Domain.Events;
using PaymentCompleted = BankingAppDDD.PaymentProcessing.Domain.Events.PaymentCompleted;

namespace BankingAppDDD.PaymentProcessing.Domain;

public class Payment : EntityBase, IAggregateRoot
{
    public int TransactionNumber { get; set; }
    public decimal Amount { get; set; }
    public TransferType Method { get; set; }
    public int SourceAccountNo { get; set; }
    public int DestinationAccountNo { get; set; }
    public string IfscCode { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public string CurrencyCode { get; init; } = "INR";
    public TransferStatus Status { get; private set; }
    public DateTime? CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? CanceledAt { get; private set; }

    public static Payment Create(PaymentData paymentData)
    {
        if (paymentData == null)
            throw new ArgumentNullException(nameof(paymentData));

        if (paymentData.TransactionNumber == 0)
            throw new BusinessRuleException("The TransactionNumber is required.");

        if (paymentData.SourceAccountNo == 0)
            throw new BusinessRuleException("The SourceAccountNo is required.");

        if (paymentData.Amount == 0m)
            throw new BusinessRuleException("The amount is required.");

        return new Payment(paymentData);
    }

    public void Complete()
    {
        if (Status != TransferStatus.Pending)
            throw new BusinessRuleException($"Payment cannot be completed when '{Status}'");

        var @event = PaymentCompleted.Create(Id);
        AddDomainEvent(@event);
        Apply(@event);
    }

    public void Cancel()
    {
        if (Status == TransferStatus.Failed)
            throw new BusinessRuleException($"Payment cannot be canceled when '{Status}'");

        var @event = PaymentCanceled.Create(Id, PaymentCancellationReason.ProcessmentError);
        AddDomainEvent(@event);
        Apply(@event);
    }

    private void Apply(PaymentCreated @event)
    {
        Status = TransferStatus.Pending;
        Id = @event.PaymentId;
        Amount = @event.TotalAmount;
        CreatedAt = @event.Timestamp;
    }

    private void Apply(PaymentCompleted @event)
    {
        Status = TransferStatus.Completed;
        CompletedAt = @event.Timestamp;
    }

    private void Apply(PaymentCanceled @event)
    {
        Status = TransferStatus.Failed;
        CanceledAt = @event.Timestamp;
    }

    private Payment(PaymentData paymentData)
    {
        Id = Guid.NewGuid();
        TransactionNumber = paymentData.TransactionNumber;
        Amount = paymentData.Amount;
        Method = paymentData.Method;
        SourceAccountNo = paymentData.SourceAccountNo;
        DestinationAccountNo = paymentData.DestinationAccountNo;
        IfscCode = paymentData.IfscCode ?? string.Empty;
        Remarks = paymentData.Remarks ?? string.Empty;
        CurrencyCode = paymentData.CurrencyCode ?? "INR";
        Status = TransferStatus.Pending;
        CreatedAt = DateTime.UtcNow;

        var @event = PaymentCreated.Create(Id, Amount, CurrencyCode);
        AddDomainEvent(@event);
    }

    private Payment() {}
}
