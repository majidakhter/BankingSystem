using BankingAppDDD.Applications.Abstractions.IntegrationEvents.LoanEvents;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.UserManagement.Core.Users.Entities;
using BankingAppDDD.Domains.Abstractions.Guards;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BankingAppDDD.UserManagement.Application.Users.DomainEventHandlers
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class LoanApplicationCreatedEventHandler : IConsumer<LoanApplicationCreatedEvent>
    {
        private readonly ILogger<LoanApplicationCreatedEventHandler> _logger;
        private readonly IRepository<User> _customerRepository;
        private readonly IUnitOfWork _unitofwork;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerRepository"></param>
        /// <param name="logger"></param>
        /// <param name="unitofwork"></param>
        public LoanApplicationCreatedEventHandler(IRepository<User> customerRepository,
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
            var newUser = await _customerRepository.GetByIdAsync(message.CorrelationId);
            Guard.Against.NotFound(newUser);
            newUser!.UpdateLoanApplicationStatus(message.status);
            _customerRepository.Update(newUser);
            await _unitofwork.CommitAsync();
            await Task.CompletedTask;
        }
    }
}
