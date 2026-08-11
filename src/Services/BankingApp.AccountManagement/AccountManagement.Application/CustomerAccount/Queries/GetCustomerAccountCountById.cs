using AutoMapper;
using BankingApp.AccountManagement.Application.CustomerAccounts.Models;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Domains.CustomerAccounts.Entities;

namespace BankingApp.AccountManagement.Application.CustomerAccounts.Queries
{
    public sealed record GetCustomerAccountCountById(Guid customerid) : Query<CustomerAccountDTO>;

    public sealed class GetCustomerAccountCountByIdHandler : QueryHandler<GetCustomerAccountCountById, CustomerAccountDTO>
    {
        private readonly IAccountRepository<UserAccount> _repository;

        public GetCustomerAccountCountByIdHandler(IMapper mapper,
            IAccountRepository<UserAccount> repository) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<CustomerAccountDTO> HandleAsync(GetCustomerAccountCountById request)
        {
            var customer = await _repository.GetByIdAsync(request.customerid);
            return Mapper.Map<CustomerAccountDTO>(customer);
        }
    }
}
