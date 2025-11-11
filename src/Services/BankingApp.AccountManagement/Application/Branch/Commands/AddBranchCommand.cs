using BankingApp.AccountManagement.Core.Branches.Entities;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Abstractions.Models;

namespace BankingApp.AccountManagement.Application.Branches.Commands
{
    public sealed record AddBranchCommand(string name, string branchCode, Guid bankId, string phoneNumber, AddressData address) : CreateCommand;
    public sealed class AddBranchCommandHandler : CreateCommandHandler<AddBranchCommand>
    {
        private readonly IAccountRepository<Branch> _repository;
        private readonly ILogger<AddBranchCommandHandler> _logger;
        public AddBranchCommandHandler(IAccountRepository<Branch> repository, ILogger<AddBranchCommandHandler> logger,
        IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
            _logger = logger;
        }

        protected override async Task<Guid> HandleAsync(AddBranchCommand request)
        {
            var addressData = new AddressData(request.address.street, request.address.city, request.address.state, request.address.zipCode, request.address.country);
            var created = Branch.Create(request.name, request.branchCode,request.bankId, request.phoneNumber, addressData);
            _repository.Insert(created);
            await UnitOfWork.CommitAsync();
            _logger.LogInformation("Branch Created {@event}", created.Id);
            return created.Id;
        }
    }
}
