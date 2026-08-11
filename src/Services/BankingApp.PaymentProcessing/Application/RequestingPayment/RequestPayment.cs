using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.PaymentProcessing.Model;

namespace BankingAppDDD.PaymentProcessing.Application.RequestingPayment;

public  class RequestPayment : EntityBase
{
	//public CustomerId CustomerId { get; private set; }
    public int TransactionNumber { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public int SourceAccountNo { get; set; }
    public int DestinationAccountNo { get; set; } // Bank Account Number or UPI ID (VPA)
    public string IfscCode { get; set; }          // Required for NEFT / RTGS
    public string Remarks { get; set; }
    public string CurrencyCode { get; init; } = "INR";

    public static RequestPayment Create(
    int transactionNumber,
    decimal amount,
    PaymentMethod method,
    int sourceAccountNo,
    int destinationAccountNo,
    string ifscCode,
    string remarks,
    string currencyCode)
	{
		if (transactionNumber==0)
			throw new ArgumentNullException(nameof(transactionNumber));
		if (amount==0m)
			throw new ArgumentNullException(nameof(amount));
		if (destinationAccountNo==0)
			throw new ArgumentNullException(nameof(destinationAccountNo));
		if (sourceAccountNo==0)
			throw new ArgumentNullException(nameof(sourceAccountNo));
		if (string.IsNullOrEmpty(ifscCode))
			throw new ArgumentOutOfRangeException(nameof(ifscCode));

		return new RequestPayment(transactionNumber, amount, method, sourceAccountNo, destinationAccountNo, ifscCode, remarks, currencyCode);
	}

	private RequestPayment(
    int transactionNumber,
    decimal amount,
    PaymentMethod method,
    int sourceAccountNo,
    int destinationAccountNo,
    string ifscCode,
    string remarks,
    string currencyCode)
	{
		SourceAccountNo = sourceAccountNo;
		DestinationAccountNo = destinationAccountNo;
		Amount = amount;
		TransactionNumber = transactionNumber;
		IfscCode = ifscCode;
        Remarks = remarks;
        CurrencyCode = currencyCode;
    }
}