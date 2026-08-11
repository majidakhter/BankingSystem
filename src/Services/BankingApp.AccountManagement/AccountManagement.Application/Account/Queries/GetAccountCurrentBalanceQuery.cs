using AutoMapper;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Domains.Accounts.Entities;

namespace BankingApp.AccountManagement.Application.Accounts.Queries
{
    public sealed record GetAccountCurrentBalanceQuery(Guid AccountId) : Query<decimal>;

    public sealed class GetAccountCurrentBalanceQueryHandler : QueryHandler<GetAccountCurrentBalanceQuery, decimal>
    {
        private readonly IAccountRepository<Account> _repository;

        public GetAccountCurrentBalanceQueryHandler(IMapper mapper,
            IAccountRepository<Account> repository) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<decimal> HandleAsync(GetAccountCurrentBalanceQuery request)
        {
            var account = await _repository.GetEntityById(request.AccountId);
            return account!.GetCurrentBalance().Value;
        }

    }
}
