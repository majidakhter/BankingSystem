using BankingAppDDD.Applications.Abstractions.IntegrationEvents.LoanEvents;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.CustomerManagement.Core.Customers.Entities;
using BankingAppDDD.Domains.Abstractions.Guards;
using MassTransit;

namespace BankingAppDDD.CustomerManagement.Application.Customers.DomainEventHandlers
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class LoanApplicationCreatedEventHandler : IConsumer<LoanApplicationCreatedEvent>
    {
        private readonly ILogger<LoanApplicationCreatedEventHandler> _logger;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IUnitOfWork _unitofwork;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerRepository"></param>
        /// <param name="logger"></param>
        /// <param name="unitofwork"></param>
        public LoanApplicationCreatedEventHandler(IRepository<Customer> customerRepository,
            ILogger<LoanApplicationCreatedEventHandler> logger, IUnitOfWork unitofwork)
        {
            _customerRepository = customerRepository;
            _logger = logger;
            _unitofwork = unitofwork;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<LoanApplicationCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Received loan application status event: {@Event}", message);
            var newCustomer = await _customerRepository.GetByIdAsync(message.CorrelationId);
            Guard.Against.NotFound(newCustomer);
            newCustomer!.UpdateLoanApplicationStatus(message.status);
            _customerRepository.Update(newCustomer);
            await _unitofwork.CommitAsync();
            await Task.CompletedTask;
        }
    }
}
