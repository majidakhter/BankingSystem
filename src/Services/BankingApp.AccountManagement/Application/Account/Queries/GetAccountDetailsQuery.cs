using AutoMapper;
using BankingApp.AccountManagement.Application.Accounts.Models;
using BankingApp.AccountManagement.Application.Banks.Queries;
using BankingApp.AccountManagement.Core.Accounts.Entities;
using BankingApp.AccountManagement.Core.Banks.Entities;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.AccountManagement.Application.Accounts.Queries
{
    
    public sealed record GetAccountDetailsQuery() : Query<List<AccountDTO>>;
    public sealed class GetAccountDetailsQueryHandler : QueryHandler<GetAccountDetailsQuery, List<AccountDTO>>
    {
        private readonly IAccountRepository<Account> _repository;

        public GetAccountDetailsQueryHandler(IMapper mapper,
            IAccountRepository<Account> repository) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<List<AccountDTO>> HandleAsync(GetAccountDetailsQuery request)
        {
            var banks = await _repository.GetAll().ToListAsync();
            return Mapper.Map<List<AccountDTO>>(banks);
        }
    }
}
