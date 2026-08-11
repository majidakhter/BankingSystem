using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Abstractions.Models;
using BankingAppDDD.Domains.Branches.Entities;
using BankingAppDDD.MongoService.Application.Mongo;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Branches.Commands
{
    public sealed record AddBranchCommand(string name, string branchCode, string ifscCode, int micrCode, Guid bankId, string phoneNumber, AddressData address) : CreateCommand;
    public sealed class AddBranchCommandHandler : CreateCommandHandler<AddBranchCommand>
    {
        private readonly IAccountRepository<Branch> _repository;
        private readonly ILogger<AddBranchCommandHandler> _logger;
        private readonly IAccountMongoService _accountMongoService;
        public AddBranchCommandHandler(IAccountRepository<Branch> repository, IAccountMongoService accountMongoService, ILogger<AddBranchCommandHandler> logger,
        IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
            _accountMongoService = accountMongoService;
            _logger = logger;
        }

        protected override async Task<Guid> HandleAsync(AddBranchCommand request)
        {
            var addressData = new AddressData(request.address.street, request.address.city, request.address.state, request.address.zipCode, request.address.country);
            var created = Branch.Create(request.name, request.branchCode,request.ifscCode,request.micrCode, request.bankId, request.phoneNumber, addressData);
            _repository.Insert(created);
            await UnitOfWork.CommitAsync();
            await _accountMongoService.SaveBranchDetailAsync(created);
            _logger.LogInformation("Branch Created {@event}", created.Id);
            return created.Id;
        }
    }
}
