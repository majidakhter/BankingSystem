using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Abstractions.Guards;
using BankingAppDDD.Domains.Accounts.Entities;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Accounts.Commands
{
    public sealed record WithdrawCommand(Guid accountId, decimal amount, string description) : Command;

    public sealed class WithdrawCommandHandler : CommandHandler<WithdrawCommand>
    {
        private readonly IAccountRepository<Account> _repository;
        private readonly ILogger<WithdrawCommandHandler> _logger;
        public WithdrawCommandHandler(IAccountRepository<Account> repository, ILogger<WithdrawCommandHandler> logger, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
            _logger = logger;
        }

        protected override async Task<bool> HandleAsync(WithdrawCommand request)
        {
            var account = await _repository.GetEntityById(request.accountId);
            Guard.Against.NotFound(account);
            account.Withdraw(request.accountId,request.amount, request.description);
            _repository.Update(account);
            await UnitOfWork.CommitAsync();
            _logger.LogInformation("Amount Withdrawn from {@Account No}", request.accountId);
            return true;
        }
    }
}
