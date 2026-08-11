using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.UserManagement.Core.Users.Entities;
using BankingAppDDD.Domains.Abstractions.Models;
using BankingAppDDD.Domains.Users.Models;

namespace BankingAppDDD.UserManagement.Application.Users.Commands
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="customerData"></param>
    /// <param name="addressData"></param>
    public sealed record UpdateUserCommand(UserUpdateData userData, AddressData addressData) : Command;
    /// <summary>
    /// 
    /// </summary>
    public sealed class UpdateUserCommandHandler : CommandHandler<UpdateUserCommand>
    {

        private readonly IRepository<User> _repository;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="unitOfWork"></param>
        public UpdateUserCommandHandler(
            IRepository<User> repository,
           IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected override async Task<bool> HandleAsync(UpdateUserCommand request)
        {
            var user = await _repository.GetByIdAsync(request.userData.UserId);
            if (user == null)
            {
                // Handle not found scenario
                throw new InvalidOperationException($"User with ID {request.userData.UserId} not found.");
            }

            user.UpdateInformation(request.userData, request.addressData);
            _repository.Update(user);
            await UnitOfWork.CommitAsync();
            return true;
        }
    }

}
