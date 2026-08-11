using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Applications.Abstractions.Shared.Events;
using BankingAppDDD.Common.Model;
using BankingAppDDD.Common.Polly;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.Domains.CustomerAccounts.Entities;
using BankingAppDDD.MongoService.Application.Mongo;
using MassTransit;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BankingAppDDD.AccountManagement.Application.Accounts.DomainEventHandlers.AccountCreated
{
    public sealed class AccountProvisionedDomainEventHandler : IConsumer<BankAccountProvisionedEvent>
    {
        private readonly IAccountMongoService _mongoService;
        private readonly ILogger<AccountProvisionedDomainEventHandler> _logger;
        private readonly IAccountRepository<Account> _accountRepository;
        private readonly IAccountRepository<UserAccount> _customerRepository;
        IOptionsMonitor<PolyConfigSettings> _policyConfigSettings;
        private readonly IUnitOfWork _unitofwork;
        private const string TypeName = "AccountProvisionedDomainEventHandler";
        public AccountProvisionedDomainEventHandler(
            IAccountRepository<UserAccount> customerRepository,
            IAccountRepository<Account> accountRepository,
            ILogger<AccountProvisionedDomainEventHandler> logger,
            IOptionsMonitor<PolyConfigSettings> policyConfigSettings,
            IUnitOfWork unitofwork,
            IAccountMongoService mongoService)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            _logger = logger;
            _mongoService = mongoService;
            _unitofwork = unitofwork;
            _policyConfigSettings = policyConfigSettings;
        }

        public async Task Consume(ConsumeContext<BankAccountProvisionedEvent> context)
        {
            try
            {
                var message = context.Message;
                _logger.LogInformation("Received customer created event: {@Event}", message);
                var accountcreated = Account.Create(message.UserId, message.KeyCloakUserId, message.AccounTypeId);
                accountcreated.Deposit(accountcreated.Id, message.InitialBalance, "initial amount deposited");
                _accountRepository.Insert(accountcreated);
                
                var existingCustomerAccount = await _customerRepository.FirstOrDefaultAsync(u => u.UserId == message.UserId || u.UserId == message.KeyCloakUserId);
                if (existingCustomerAccount != null)
                {
                    existingCustomerAccount.SetOneAccountAdded();
                    _customerRepository.Update(existingCustomerAccount);
                }
                else
                {
                    var newCustomeraccount = UserAccount.Create(message.UserId);
                    newCustomeraccount.SetOneAccountAdded();
                    _customerRepository.Insert(newCustomeraccount);
                }
                var pollyWrapper = new PollyWrap<Account>(_policyConfigSettings, _logger);
                var wrapped = pollyWrapper.GetPolicyConfig(accountcreated, TypeName);
                await wrapped.ExecuteAsync(async () =>
                {
                    await _unitofwork!.CommitAsync();
                    if (_mongoService != null)
                    {
                        await _mongoService.SaveAccountDetailAsync(accountcreated);
                    }
                }).ConfigureAwait(false);
                
               _logger.LogInformation("Saved account detail to MongoService for AccountId: {AccountId}", accountcreated.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing BankAccountProvisionedEvent in AccountProvisionedDomainEventHandler");
                throw;
            }
        }
    }
}
