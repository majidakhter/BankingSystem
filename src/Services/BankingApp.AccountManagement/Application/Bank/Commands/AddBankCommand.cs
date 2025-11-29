using BankingApp.AccountManagement.Core.Banks.Entities;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Abstractions.Models;

namespace BankingApp.AccountManagement.Application.Banks.Commands
{
    public sealed record AddBankCommand(string name, string phoneNumber, AddressData address) : CreateCommand;
    public sealed class AddBankCommandHandler : CreateCommandHandler<AddBankCommand>
    {
        private readonly IAccountRepository<Bank> _repository;
        private readonly ILogger<AddBankCommandHandler> _logger;
        public AddBankCommandHandler(IAccountRepository<Bank> repository, ILogger<AddBankCommandHandler> logger,
        IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
            _logger = logger;
        }

        protected override async Task<Guid> HandleAsync(AddBankCommand request)
        {
            DateTime localDateTime = DateTime.Now;
            var created = Bank.Create(request.name, request.phoneNumber, request.address, localDateTime.ToUniversalTime());
            _repository.Insert(created);
            await UnitOfWork.CommitAsync();
            _logger.LogInformation("Bank Created {@event}", created.Id);
            return created.Id;
        }
    }
}
