using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.CustomerManagement.Application.Customers.Models;
using BankingAppDDD.CustomerManagement.Core.Customers.Entities;
using BankingAppDDD.Domains.Abstractions.Models;

namespace BankingAppDDD.CustomerManagement.Application.Customers.Commands
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="customerData"></param>
    /// <param name="addressData"></param>
    public sealed record UpdateCustomerCommand(CustomerUpdateData customerData, AddressData addressData) : Command;
    /// <summary>
    /// 
    /// </summary>
    public sealed class UpdateCustomerCommandHandler : CommandHandler<UpdateCustomerCommand>
    {

        private readonly IRepository<Customer> _repository;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="unitOfWork"></param>
        public UpdateCustomerCommandHandler(
            IRepository<Customer> repository,
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
        protected override async Task<bool> HandleAsync(UpdateCustomerCommand request)
        {
            var customer = await _repository.GetByIdAsync(request.customerData.CustomerId);
            if (customer == null)
            {
                // Handle not found scenario
                throw new InvalidOperationException($"Customer with ID {request.customerData.CustomerId} not found.");
            }

            customer.UpdateInformation(request.customerData, request.addressData);
            _repository.Update(customer);
            await UnitOfWork.CommitAsync();
            return true;
        }
    }

}
