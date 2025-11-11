using AutoMapper;
using BankingApp.AccountManagement.Application.Branches.Model;
using BankingApp.AccountManagement.Core.Branches.Entities;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.AccountManagement.Application.Branches.Queries
{
    public sealed record GetBranchQuery() : Query<List<BranchDTO>>;

    public sealed class GetBranchQueryHandler : QueryHandler<GetBranchQuery, List<BranchDTO>>
    {
        private readonly IAccountRepository<Branch> _repository;

        public GetBranchQueryHandler(IMapper mapper,
            IAccountRepository<Branch> repository) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<List<BranchDTO>> HandleAsync(GetBranchQuery request)
        {
            var bank = await _repository.GetAll().ToListAsync();
            return Mapper.Map<List<BranchDTO>>(bank);
        }
    }
}
