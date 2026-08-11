using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Banks.Entities;
using BankingAppDDD.MongoService.Application.Mongo;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Banks.Commands
{
    public sealed record AddBankCommand(string name) : CreateCommand;
    public sealed class AddBankCommandHandler : CreateCommandHandler<AddBankCommand>
    {
        private readonly IAccountRepository<Bank> _repository;
        private readonly ILogger<AddBankCommandHandler> _logger;
        private readonly IAccountMongoService _accountMongoService;
        public AddBankCommandHandler(IAccountRepository<Bank> repository, IAccountMongoService accountMongoService, ILogger<AddBankCommandHandler> logger,
        IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
            _accountMongoService = accountMongoService;
            _logger = logger;
        }

        protected override async Task<Guid> HandleAsync(AddBankCommand request)
        {
            DateTime localDateTime = DateTime.Now;
            var created = Bank.Create(request.name, localDateTime.ToUniversalTime());
            _repository.Insert(created);
            await UnitOfWork.CommitAsync();
            await _accountMongoService.SaveBankDetailAsync(created);
            _logger.LogInformation("Bank Created {@event}", created.Id);
            return created.Id;
        }
    }
}
