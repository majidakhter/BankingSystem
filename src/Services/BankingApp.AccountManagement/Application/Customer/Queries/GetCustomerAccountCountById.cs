using AutoMapper;
using BankingApp.AccountManagement.Application.Customers.Models;
using BankingApp.AccountManagement.Core.Customers.Entities;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Applications.Abstractions.Repositories;

namespace BankingApp.AccountManagement.Application.Customers.Queries
{
    public sealed record GetCustomerAccountCountById(Guid customerid) : Query<CustomerAccountDTO>;

    public sealed class GetCustomerAccountCountByIdHandler : QueryHandler<GetCustomerAccountCountById, CustomerAccountDTO>
    {
        private readonly IRepository<Customer> _repository;

        public GetCustomerAccountCountByIdHandler(IMapper mapper,
            IRepository<Customer> repository) : base(mapper)
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
