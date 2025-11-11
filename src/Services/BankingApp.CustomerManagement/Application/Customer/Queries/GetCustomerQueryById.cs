using AutoMapper;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.CustomerManagement.Application.Customers.Models;
using BankingAppDDD.CustomerManagement.Core.Customers.Entities;

namespace BankingAppDDD.CustomerManagement.Application.Customers.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    public sealed record GetCustomerQueryById(Guid id) : Query<CustomerDTO>;
    /// <summary>
    /// 
    /// </summary>
    public sealed class GetCustomerQueryByIdHandler : QueryHandler<GetCustomerQueryById, CustomerDTO>
    {
        private readonly IRepository<Customer> _repository;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="repository"></param>
        public GetCustomerQueryByIdHandler(IMapper mapper,
            IRepository<Customer> repository) : base(mapper)
        {
            _repository = repository;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected override async Task<CustomerDTO> HandleAsync(GetCustomerQueryById request)
        {
            var customer = await _repository.GetByIdAsync(request.id);
            return Mapper.Map<CustomerDTO>(customer);
        }
    }
}
