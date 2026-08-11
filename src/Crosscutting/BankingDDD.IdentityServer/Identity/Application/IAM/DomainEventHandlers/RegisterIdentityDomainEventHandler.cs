using BankingAppDDD.Applications.Abstractions.DomainEventHandlers;
using BankingAppDDD.Applications.Abstractions.Shared.Commands;
using BankingAppDDD.Applications.Abstractions.Shared.Events;
using MassTransit;

namespace Identity.Application.IAM.DomainEventHandlers
{
    public class RegisterIdentityDomainEventHandler : DomainEventHandler<UserIdentityCreatedEvent>
    {
        private readonly ILogger<RegisterIdentityDomainEventHandler> _logger;
        private readonly IBus _eventBus;

        public RegisterIdentityDomainEventHandler(
            ILogger<RegisterIdentityDomainEventHandler> logger,
            IBus eventBus) : base(logger)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        protected override async Task OnHandleAsync(UserIdentityCreatedEvent request)
        {
            _logger.LogInformation("Handling UserIdentityCreatedEvent for KeycloakUserId: {KeycloakUserId}", request.KeycloakUserId);

            // Publish CreateUserProfileCommand to MassTransit bus as part of choreography pipeline
            var command = new CreateUserProfileCommand(request.UserData, request.AddressData, request.KeycloakUserId, request.AccountTypeId, request.OpeningBalance, request.BranchId);
            await _eventBus.Publish(command);


            _logger.LogInformation("Published CreateUserProfileCommand to MassTransit bus for KeycloakUserId: {KeycloakUserId}", request.KeycloakUserId);
        }
    }
}
