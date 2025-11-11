using AutoMapper;
using BankingApp.AccountManagement.Application.Branches.Model;
using BankingApp.AccountManagement.Core.Branches.Entities;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;

namespace BankingApp.AccountManagement.Application.Branches.Queries
{
    public sealed record GetBranchQueryById(Guid id) : Query<BranchDTO>;

    public sealed class GetBranchQueryByIdHandler : QueryHandler<GetBranchQueryById, BranchDTO>
    {
        private readonly IAccountRepository<Branch> _repository;

        public GetBranchQueryByIdHandler(IMapper mapper,
            IAccountRepository<Branch> repository) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<BranchDTO> HandleAsync(GetBranchQueryById request)
        {
            var bank = await _repository.GetByIdAsync(request.id);
            return Mapper.Map<BranchDTO>(bank);
        }
    }
}
