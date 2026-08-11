using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Accounts.Models;
using BankingAppDDD.PaymentProcessing.Domain;
using Microsoft.Extensions.Logging;

namespace BankingAppDDD.PaymentProcessing.Application.RequestingPayment;

public sealed record RequestPaymentCommand(
    int TransactionNumber,
    decimal Amount,
    TransferType Method,
    int SourceAccountNo, 
    int DestinationAccountNo,
    string IfscCode,
    string Remarks,
    string CurrencyCode) : Command;

public sealed class RequestPaymentCommandHandler : CommandHandler<RequestPaymentCommand>
{
    private readonly ILogger<RequestPaymentCommandHandler> _logger;

    public RequestPaymentCommandHandler(
        ILogger<RequestPaymentCommandHandler> logger,
        IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        _logger = logger;
    }

    protected override async Task<bool> HandleAsync(RequestPaymentCommand request)
    {
        _logger.LogInformation("Processing RequestPaymentCommand for TxnNo: {TransactionNo}", request.TransactionNumber);
        await Task.CompletedTask;
        return true;
    }
}
