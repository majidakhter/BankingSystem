namespace BankingAppDDD.PaymentProcessing.Application.ProcessingPayment;

public record class ProcessPayment
{
    public Guid PaymentId { get; private set; }

    public static ProcessPayment Create(Guid paymentId)
    {
        return new ProcessPayment { PaymentId = paymentId };
    }
}