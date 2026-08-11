using BankingAppDDD.Domains.Abstractions.DomainEvents;
using BankingAppDDD.Domains.Abstractions.Models;

namespace BankingAppDDD.Applications.Abstractions.Shared.Events
{
    public record class UserIdentityCreatedEvent : DomainEvent
    {
        public UserIdentityData UserData { get; private set; }
        public AddressData AddressData { get; private set; }
        public Guid KeycloakUserId { get; private set; }
        public int AccountTypeId { get; private set; }
        public decimal OpeningBalance { get; private set; }
        public Guid? BranchId { get; private set; }

        public static UserIdentityCreatedEvent Create(
            UserIdentityData userData,
            AddressData permanentAddress,
            Guid keycloakUserId,
            int accountTypeId,
            decimal openingBalance,
            Guid? branchId = null)
        {
            if (keycloakUserId == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(keycloakUserId));
            return new UserIdentityCreatedEvent(
                userData,
                permanentAddress,
                keycloakUserId,
                accountTypeId,
                openingBalance,
                branchId);
        }

        private UserIdentityCreatedEvent(
            UserIdentityData userData,
            AddressData permanentAddress,
            Guid keycloakUserId,
            int accountTypeId,
            decimal openingBalance,
            Guid? branchId = null)
        {
            UserData = userData;
            AddressData = permanentAddress;
            KeycloakUserId = keycloakUserId;
            AccountTypeId = accountTypeId;
            OpeningBalance = openingBalance;
            BranchId = branchId;
        }
    }
}
