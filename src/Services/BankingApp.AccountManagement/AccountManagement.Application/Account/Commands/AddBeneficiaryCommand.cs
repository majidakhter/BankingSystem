using System;
using System.Text.Json.Serialization;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Abstractions.Guards;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.Domains.Accounts.Models;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Accounts.Commands
{
    public sealed record AddBeneficiaryCommand(
        Guid accountId, 
        string beneficiaryName, 
        [property: JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)] int beneficiaryAccountNo, 
        string beneficiaryBankName
    ) : Command;

    public sealed class AddBeneficiaryCommandHandler : CommandHandler<AddBeneficiaryCommand>
    {
        private readonly IAccountRepository<Account> _repository;
        private readonly IRepository<BeneficiaryGroup> _beneficiaryRepository;
        private readonly ILogger<AddBeneficiaryCommandHandler> _logger;

        public AddBeneficiaryCommandHandler(
            IAccountRepository<Account> repository,
            IRepository<BeneficiaryGroup> beneficiaryRepository,
            ILogger<AddBeneficiaryCommandHandler> logger,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
            _beneficiaryRepository = beneficiaryRepository;
            _logger = logger;
        }

        protected override async Task<bool> HandleAsync(AddBeneficiaryCommand request)
        {
            var account = await _repository.GetEntityById(request.accountId);
            if (account == null)
            {
                account = await _repository.FirstOrDefaultAsync(a => a.UserId == request.accountId || a.KeycloakUserId == request.accountId);
            }
            Guard.Against.NotFound(account);

            var beneficiaryData = new BeneficiaryData(request.beneficiaryName, request.beneficiaryAccountNo, request.beneficiaryBankName);
            var group = account!.AddBeneficiary(beneficiaryData, account.Id);

            _beneficiaryRepository.Insert(group);
            await UnitOfWork.CommitAsync();

            _logger.LogInformation("Added Beneficiary for AccountId {AccountId}: {@BeneficiaryName}", account.Id, request.beneficiaryName);
            return true;
        }
    }
}
