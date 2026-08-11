using BankingAppDDD.Domains.Abstractions.Models;

namespace BankingAppDDD.Applications.Abstractions.Shared.Events
{
    public record UserProfileCreatedEvent(
        UserIdentityData userdata,
        AddressData AddressData,
        Guid UserId,
        Guid KeyCloakUserId,
        int accountTypeId,
        decimal amount
    );
}
