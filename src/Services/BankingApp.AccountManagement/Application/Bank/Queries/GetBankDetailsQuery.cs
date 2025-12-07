using AutoMapper;
using BankingApp.AccountManagement.Application.Banks.Models;
using BankingApp.AccountManagement.Core.Banks.Entities;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;
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
