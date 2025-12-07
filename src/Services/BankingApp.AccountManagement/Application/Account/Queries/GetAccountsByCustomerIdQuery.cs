
using AutoMapper;
using BankingApp.AccountManagement.Application.Accounts.Models;
using BankingApp.AccountManagement.Application.Accounts.Queries;
using BankingApp.AccountManagement.Core.Accounts.Entities;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;
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
            var accounts = await _repository.GetAll().ToListAsync();
            var filteredaccounts = accounts.Where(x=>x.CustomerId == request.CustomerId);
            return Mapper.Map<List<AccountDTO>>(filteredaccounts);
        }
    }
}
