using AutoMapper;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.Domains.Accounts.Models;
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
            var accounts= await _repository.GetAll().ToListAsync();
            return Mapper.Map<List<AccountDTO>>(accounts);
        }
    }
}
