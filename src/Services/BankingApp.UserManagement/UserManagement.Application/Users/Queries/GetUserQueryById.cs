using AutoMapper;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.UserManagement.Core.Users.Entities;
using BankingAppDDD.Domains.Users.Models;

namespace BankingAppDDD.UserManagement.Application.Users.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    public sealed record GetUserQueryById(Guid id) : Query<UserDTO>;
    /// <summary>
    /// 
    /// </summary>
    public sealed class GetCustomerQueryByIdHandler : QueryHandler<GetUserQueryById, UserDTO>
    {
        private readonly IRepository<User> _repository;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="repository"></param>
        public GetCustomerQueryByIdHandler(IMapper mapper,
            IRepository<User> repository) : base(mapper)
        {
            _repository = repository;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected override async Task<UserDTO> HandleAsync(GetUserQueryById request)
        {
            var customer = await _repository.GetByIdAsync(request.id);
            return Mapper.Map<UserDTO>(customer);
        }
    }
}
