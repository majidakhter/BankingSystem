using AutoMapper;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.CustomerManagement.Application.Customers.Models;
using BankingAppDDD.CustomerManagement.Core.Customers.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingAppDDD.CustomerManagement.Application.Customers.Queries
{
    /// <summary>
    /// 
    /// </summary>
    public sealed record GetCustomerQuery() : Query<List<CustomerDTO>>;
    /// <summary>
    /// 
    /// </summary>
    public sealed class GetCustomerQueryHandler : QueryHandler<GetCustomerQuery, List<CustomerDTO>>
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IRepository<Customer> _repository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="repository"></param>
        public GetCustomerQueryHandler(IMapper mapper,
            IRepository<Customer> repository) : base(mapper)
        {
            _repository = repository;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected override async Task<List<CustomerDTO>> HandleAsync(GetCustomerQuery request)
        {
            var customers = await _repository.GetAll().ToListAsync();
            return Mapper.Map<List<CustomerDTO>>(customers);
        }
    }
}
