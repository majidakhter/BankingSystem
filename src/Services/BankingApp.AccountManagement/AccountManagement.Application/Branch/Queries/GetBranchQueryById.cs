using AutoMapper;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Domains.Branches.Entities;
using BankingAppDDD.Domains.Branches.Model;

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
