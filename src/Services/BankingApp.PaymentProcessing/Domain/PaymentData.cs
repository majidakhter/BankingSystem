using BankingAppDDD.Domains.Accounts.Models;

namespace BankingAppDDD.PaymentProcessing.Domain;

public record class PaymentData(
    int TransactionNumber,
    decimal Amount,
    TransferType Method,
    int SourceAccountNo,
    int DestinationAccountNo,
    string IfscCode,
    string Remarks,
    string CurrencyCode);