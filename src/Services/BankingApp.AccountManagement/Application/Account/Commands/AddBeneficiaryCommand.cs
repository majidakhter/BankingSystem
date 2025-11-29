using BankingApp.AccountManagement.Core.Accounts.Entities;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Abstractions.Guards;

namespace BankingApp.AccountManagement.Application.Accounts.Commands
{
    public sealed record AddBeneficiaryCommand(Guid accountId, string beneficiaryName, int beneficiaryAccount, string beneficiaryBankName) : Command;
    public sealed class AddBeneficiaryCommandHandler : CommandHandler<AddBeneficiaryCommand>
    {
        private readonly IAccountRepository<Account> _repository;
        private readonly ILogger<AddBeneficiaryCommandHandler> _logger;
        public AddBeneficiaryCommandHandler(IAccountRepository<Account> repository, ILogger<AddBeneficiaryCommandHandler> logger, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
            _logger = logger;
        }

        protected override async Task<bool> HandleAsync(AddBeneficiaryCommand request)
        {
            var account = await _repository.GetByIdAsync(request.accountId);
            var beneficiary = new BeneficiaryData(request.beneficiaryName, request.beneficiaryAccount, request.beneficiaryBankName);
            Guard.Against.NotFound(account);
            account!.AddBeneficiary(beneficiary);
            _repository.Update(account);
            await UnitOfWork.CommitAsync();
            _logger.LogInformation("Added Beneficiary : {@Event}", request.beneficiaryName);
            return true;
        }
    }
}
