using BankingAppDDD.Domains.Abstractions.DomainEvents;

namespace BankingAppDDD.PaymentProcessing.Domain.Events;

public record PaymentCreated : DomainEvent
{
    public Guid PaymentId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string CurrencyCode { get; private set; }

    public static PaymentCreated Create(
        Guid paymentId,
        decimal totalAmount,
        string currencyCode)
    {
        return new PaymentCreated
        {
            PaymentId = paymentId,
            TotalAmount = totalAmount,
            CurrencyCode = currencyCode
        };
    }
}