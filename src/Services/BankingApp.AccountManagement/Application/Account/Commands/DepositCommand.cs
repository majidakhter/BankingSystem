using BankingApp.AccountManagement.Core.Accounts.Entities;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;

namespace BankingApp.AccountManagement.Application.Accounts.Commands
{
    public sealed record DepositCommand(Guid accountId, decimal amount) : Command;
    public sealed class DepositCommandHandler : CommandHandler<DepositCommand>
    {
        private readonly IAccountRepository<Account> _repository;
        private readonly ILogger<DepositCommandHandler> _logger;
        public DepositCommandHandler(IAccountRepository<Account> repository, ILogger<DepositCommandHandler> logger, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
            _logger = logger;
        }

        protected override async Task<bool> HandleAsync(DepositCommand request)
        {

            var account = await _repository.GetEntityById(request.accountId);
            if (account == null)
            {
                _logger.LogInformation("AccountId does not exist: {@Account No}", request.accountId);
                throw new ArgumentException("AccountId does not exist");
            }
            account.Deposit(request.amount);
            _repository.Update(account);
            await UnitOfWork.CommitAsync();
            _logger.LogInformation("Amount Deposited to {@Account No}", request.accountId);
            return true;
        }
    }
}
