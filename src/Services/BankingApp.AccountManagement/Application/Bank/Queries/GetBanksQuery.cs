using AutoMapper;
using BankingApp.AccountManagement.Application.Banks.Models;
using BankingApp.AccountManagement.Core.Banks.Entities;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;

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
