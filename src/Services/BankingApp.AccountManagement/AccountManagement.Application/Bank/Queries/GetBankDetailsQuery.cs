using AutoMapper;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Domains.Banks.Entities;
using BankingAppDDD.Domains.Banks.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.AccountManagement.Application.Banks.Queries
{
    public sealed record GetBankDetailsQuery() : Query<List<BankDTO>>;
    public sealed class GetBankDetailsQueryHandler : QueryHandler<GetBankDetailsQuery, List<BankDTO>>
    {
        private readonly IAccountRepository<Bank> _repository;

        public GetBankDetailsQueryHandler(IMapper mapper,
            IAccountRepository<Bank> repository) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<List<BankDTO>> HandleAsync(GetBankDetailsQuery request)
        {
            var banks = await _repository.GetAll().ToListAsync();
            return Mapper.Map<List<BankDTO>>(banks);
        }
    }
}
