using AutoMapper;
using BankingApp.AccountManagement.Application.Accounts.Models;
using BankingApp.AccountManagement.Core.Accounts.Entities;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Domains.Abstractions.Guards;

namespace BankingApp.AccountManagement.Application.Accounts.Queries
{
    public sealed record AccountStatusQuery(Guid AccountId) : Query<AccountStatusDTO>;

    public sealed class AccountStatusQueryHandler : QueryHandler<AccountStatusQuery, AccountStatusDTO>
    {
        private readonly IAccountRepository<Account> _repository;

        public AccountStatusQueryHandler(IMapper mapper,
            IAccountRepository<Account> repository) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<AccountStatusDTO> HandleAsync(AccountStatusQuery request)
        {
            var account = await _repository.GetByIdAsync(request.AccountId);
            Guard.Against.NotFound(account);
            return Mapper.Map<AccountStatusDTO>(account);
        }
    }
}
