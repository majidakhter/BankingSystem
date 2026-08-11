
using AutoMapper;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.Domains.Accounts.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.AccountManagement.Application.Accounts.Queries
{
    public sealed record GetAccountsByCustomerIdQuery(Guid CustomerId) : Query<List<AccountDTO>>;
    public sealed class GetAccountsByCustomerIdQueryHandler : QueryHandler<GetAccountsByCustomerIdQuery, List<AccountDTO>>
    {
        private readonly IAccountRepository<Account> _repository;

        public GetAccountsByCustomerIdQueryHandler(IMapper mapper,
            IAccountRepository<Account> repository) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<List<AccountDTO>> HandleAsync(GetAccountsByCustomerIdQuery request)
        {
            var accounts = await _repository.GetAll().Where(x => x.UserId == request.CustomerId).ToListAsync();
            return Mapper.Map<List<AccountDTO>>(accounts);
        }
    }
}
