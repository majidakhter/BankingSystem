using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Abstractions.Guards;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.MongoService.Application.Mongo;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Accounts.Commands
{
    public sealed record WithdrawCommand(int accountNumber, decimal amount, string description) : Command;

    public sealed class WithdrawCommandHandler : CommandHandler<WithdrawCommand>
    {
        private readonly IAccountRepository<Account> _repository;
        private readonly ILogger<WithdrawCommandHandler> _logger;
        private readonly IAccountMongoService? _mongoService;
        public WithdrawCommandHandler(IAccountRepository<Account> repository, IAccountMongoService? mongoService, ILogger<WithdrawCommandHandler> logger, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
            _logger = logger;
            _mongoService = mongoService;
        }

        protected override async Task<bool> HandleAsync(WithdrawCommand request)
        {
            var account = await _repository.GetEntityByAccountNumber(request.accountNumber);
            Guard.Against.NotFound(account);
            account.Withdraw(account.Id,request.amount, request.description);
            _repository.Update(account);
            if (_mongoService != null)
            {
                await _mongoService.SaveAccountDetailAsync(account);
            }
            await UnitOfWork.CommitAsync();
            _logger.LogInformation("Amount Withdrawn from {@Account No}", request.accountNumber);
            return true;
        }
    }
}
