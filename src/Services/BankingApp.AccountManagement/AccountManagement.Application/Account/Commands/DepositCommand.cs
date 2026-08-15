using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.MongoService.Application.Mongo;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Accounts.Commands
{
    public sealed record DepositCommand(int accountNumber, decimal amount, string description) : Command;
    public sealed class DepositCommandHandler : CommandHandler<DepositCommand>
    {
        private readonly IAccountRepository<Account> _repository;
        private readonly ILogger<DepositCommandHandler> _logger;
        private readonly IAccountMongoService? _mongoService;
        public DepositCommandHandler(IAccountRepository<Account> repository, IAccountMongoService? mongoService, ILogger<DepositCommandHandler> logger, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
            _logger = logger;
            _mongoService = mongoService;
        }

        protected override async Task<bool> HandleAsync(DepositCommand request)
        {

            var account = await _repository.GetEntityByAccountNumber(request.accountNumber);
            if (account == null)
            {
                _logger.LogInformation("AccountId does not exist: {@Account No}", request.accountNumber);
                throw new ArgumentException("accountNumber does not exist");
            }
            account.Deposit(account.Id, request.amount, request.description);
            _repository.Update(account);
            if (_mongoService != null)
            {
                await _mongoService.SaveAccountDetailAsync(account);
            }
            await UnitOfWork.CommitAsync();

            _logger.LogInformation("Amount Deposited to {@Account No}", request.accountNumber);
            return true;
        }
    }
}
