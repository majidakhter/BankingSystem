using BankingApp.AccountManagement.Core.Accounts.Entities;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Abstractions.Guards;
using MediatR;

namespace BankingApp.AccountManagement.Application.Accounts.Commands
{
    public sealed record CloseAccountCommand(Guid accountId, Guid customerId) : Command;
    public sealed class CloseAccountCommandHandler : CommandHandler<CloseAccountCommand>
    {

        private readonly IAccountRepository<Account> _repository;
        private readonly ILogger<CloseAccountCommandHandler> _logger;
        public CloseAccountCommandHandler(
            IAccountRepository<Account> repository, ILogger<CloseAccountCommandHandler> logger,
           IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
            _logger = logger;
        }


        protected override async Task<bool> HandleAsync(CloseAccountCommand request)
        {
            var account = await _repository.GetEntityById(request.accountId);
            Guard.Against.NotFound(account);
            account.Close(request.customerId);
            _repository.Update(account);
            await UnitOfWork.CommitAsync();
            _logger.LogInformation("Account closed for Customer request.customerId : {@Account No}", request.accountId);
            return true;
        }
    }
}
