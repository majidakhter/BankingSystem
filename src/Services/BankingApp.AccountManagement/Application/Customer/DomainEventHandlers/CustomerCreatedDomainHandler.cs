using BankingApp.AccountManagement.Core.Customers.Entities;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.IntegrationEvents.CustomerEvents;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Abstractions.Guards;
using MassTransit;

namespace BankingApp.AccountManagement.Application.Customers.DomainEventHandlers
{
    public sealed class CustomerCreatedEventHandler : IConsumer<CustomerCreatedEvent>
    {
        private readonly ILogger<CustomerCreatedEventHandler> _logger;
        private readonly IAccountRepository<Customer> _customerRepository;
        private readonly IUnitOfWork _unitofwork;
        public CustomerCreatedEventHandler(IAccountRepository<Customer> customerRepository,
            ILogger<CustomerCreatedEventHandler> logger, IUnitOfWork unitofwork)
        {
            _customerRepository = customerRepository;
            _logger = logger;
            _unitofwork = unitofwork;
        }

        public async Task Consume(ConsumeContext<CustomerCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Received customer created event: {@Event}", message);
            var newCustomer = Customer.Create(message.CorrelationId);
            Guard.Against.NotFound(newCustomer);
            _customerRepository.Insert(newCustomer);
            await _unitofwork.CommitAsync();

            await Task.CompletedTask;
        }
    }
}
