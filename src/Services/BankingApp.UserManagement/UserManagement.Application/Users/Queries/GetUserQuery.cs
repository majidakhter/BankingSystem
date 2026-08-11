using AutoMapper;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.UserManagement.Core.Users.Entities;
using Microsoft.EntityFrameworkCore;
using BankingAppDDD.Domains.Users.Models;

namespace BankingAppDDD.UserManagement.Application.Users.Queries
{
    /// <summary>
    /// 
    /// </summary>
    public sealed record GetUserQuery() : Query<List<UserDTO>>;
    /// <summary>
    /// 
    /// </summary>
    public sealed class GetCustomerQueryHandler : QueryHandler<GetUserQuery, List<UserDTO>>
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IRepository<User> _repository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="repository"></param>
        public GetCustomerQueryHandler(IMapper mapper,
            IRepository<User> repository) : base(mapper)
        {
            _repository = repository;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected override async Task<List<UserDTO>> HandleAsync(GetUserQuery request)
        {
            var customers = await _repository.GetAll().ToListAsync();
            return Mapper.Map<List<UserDTO>>(customers);
        }
    }
}
