using AutoMapper;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Domains.Banks.Entities;
using BankingAppDDD.Domains.Banks.Models;

namespace BankingApp.AccountManagement.Application.Banks.Queries
{
    public sealed record GetBanksQuery(Guid Id) : Query<BankDTO>;

    public sealed class GetBanksQueryHandler : QueryHandler<GetBanksQuery, BankDTO>
    {
        private readonly IAccountRepository<Bank> _repository;

        public GetBanksQueryHandler(IMapper mapper,
            IAccountRepository<Bank> repository) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<BankDTO> HandleAsync(GetBanksQuery request)
        {
            var bank = await _repository.GetByIdAsync(request.Id);
            return Mapper.Map<BankDTO>(bank);
        }
    }
}
