using BankingApp.AccountManagement.Core.Accounts.Entities;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;

namespace BankingApp.AccountManagement.Application.Accounts.Commands
{
    public sealed record AddAccountCommand(Guid customerId, int accountTypeId) : CreateCommand;

    public sealed class AddAccountCommandHandler : CreateCommandHandler<AddAccountCommand>
    {
        private readonly IAccountRepository<Account> _repository;
        //private static readonly HttpClient client = new HttpClient();
        private readonly ILogger<AddAccountCommandHandler> logger;
        public AddAccountCommandHandler(IAccountRepository<Account> repository, ILogger<AddAccountCommandHandler> _logger,
        IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
            logger = _logger;
        }

        protected override async Task<Guid> HandleAsync(AddAccountCommand request)
        {
            //  var data = new AccountCreatedIntegrationEvent(request.customerId);
            // var jsonContent = JsonSerializer.Serialize(data);

            //var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            // var response = await client.PostAsync("https://localhost:7155/api/EventLog/AddEventLog", content);
            // string responseBody = await response.Content.ReadAsStringAsync();
            var created = Account.Create(request.customerId, request.accountTypeId);
            //Guard.Against.NotFound(created);
            _repository.Insert(created);
            await UnitOfWork.CommitAsync();
            logger.LogInformation("Created Account: {@Account}", created.Id);
            return created.Id;
        }
    }
}
