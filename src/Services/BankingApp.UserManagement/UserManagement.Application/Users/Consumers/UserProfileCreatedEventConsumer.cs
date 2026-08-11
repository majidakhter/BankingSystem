using BankingAppDDD.Applications.Abstractions.Shared.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BankingAppDDD.UserManagement.Application.Users.Consumers
{
    /// <summary>
    /// Consumer for UserProfileCreatedEvent published after user profile creation
    /// Publishes BankAccountProvisionedEvent to trigger AccountProvisionedDomainEventHandler in AccountManagement
    /// </summary>
    public sealed class UserProfileCreatedEventConsumer : IConsumer<UserProfileCreatedEvent>
    {
        private readonly IBus _eventBus;
        private readonly ILogger<UserProfileCreatedEventConsumer> _logger;

        public UserProfileCreatedEventConsumer(IBus eventBus, ILogger<UserProfileCreatedEventConsumer> logger)
        {
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserProfileCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Consumed UserProfileCreatedEvent for KeycloakUserId: {KeycloakUserId}", message.KeyCloakUserId);

            var bankprovisionEvent = new BankAccountProvisionedEvent(message.KeyCloakUserId, message.UserId, message.accountTypeId, message.amount);
            await _eventBus.Publish(bankprovisionEvent);
        }
    }
}
