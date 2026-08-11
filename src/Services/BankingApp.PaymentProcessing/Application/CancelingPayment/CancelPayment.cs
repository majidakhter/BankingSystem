using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.PaymentProcessing.Domain;

namespace BankingAppDDD.PaymentProcessing.Application.CancelingPayment;

public  class CancelPayment : EntityBase
{
	public PaymentCancellationReason PaymentCancellationReason { get; private set; }

	public static CancelPayment Create(
		int paymentCancellationReason)
	{

		return new CancelPayment((PaymentCancellationReason)paymentCancellationReason);
	}
	private CancelPayment(
		PaymentCancellationReason paymentCancellationReason)
	{
		PaymentCancellationReason = paymentCancellationReason;
	}
}