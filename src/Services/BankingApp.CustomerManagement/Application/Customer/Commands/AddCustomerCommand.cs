using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.CustomerManagement.Application.Customers.Models;
using BankingAppDDD.CustomerManagement.Core.Customers.Entities;
using BankingAppDDD.Domains.Abstractions.Guards;
using BankingAppDDD.Domains.Abstractions.Models;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace BankingAppDDD.CustomerManagement.Application.Customers.Commands
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="custData"></param>
    /// <param name="addressData"></param>
    public sealed record AddCustomerCommand(CustomerData custData, AddressData addressData) : CreateCommand;
    /// <summary>
    /// 
    /// </summary>
    public sealed class AddCustomerCommandHandler : CreateCommandHandler<AddCustomerCommand>
    {

        private readonly IRepository<Customer> _repository;
        private readonly ILogger<AddCustomerCommandHandler> logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="_logger"></param>
        /// <param name="unitOfWork"></param>
        public AddCustomerCommandHandler(
            IRepository<Customer> repository, ILogger<AddCustomerCommandHandler> _logger,
           IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
            logger = _logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected override async Task<Guid> HandleAsync(AddCustomerCommand request)
        {
            var newCustomer = Customer.Create(request.custData, request.addressData);
            Guard.Against.NotFound(newCustomer);
            _repository.Insert(newCustomer);
            await UnitOfWork.CommitAsync();
            logger.LogInformation("Created Customer: {@Customer}", newCustomer.Id);
            return newCustomer.Id;
        }
    }
}
