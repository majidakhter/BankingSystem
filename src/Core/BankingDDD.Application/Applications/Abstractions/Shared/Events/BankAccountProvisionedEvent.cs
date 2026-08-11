namespace BankingAppDDD.Applications.Abstractions.Shared.Events
{
    public record BankAccountProvisionedEvent(
    Guid KeyCloakUserId,
    Guid UserId,
    int AccounTypeId,
    decimal InitialBalance
);
}
