using BankingAppDDD.Applications.Abstractions.Shared.Events;
using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.Models;

namespace Identity.Core.IAM.Entities
{
    public sealed class RegisterAccount: EntityBase
    {
        protected RegisterAccount()
        {

        }

        private RegisterAccount(UserIdentityData userData, AddressData addressData, Guid keycloakUserId, int accountTypeId, decimal amount, Guid? branchId = null)
        {
            var @event = UserIdentityCreatedEvent.Create(
                userData,
                addressData,
                keycloakUserId,
                accountTypeId,
                amount,
                branchId);

            AddDomainEvent(@event);
        }

        public static RegisterAccount Create(UserIdentityData userData, AddressData addressData, Guid keycloakUserId, int accountTypeId, decimal amount, Guid? branchId = null)
        {
            var account = new RegisterAccount(userData, addressData, keycloakUserId, accountTypeId, amount, branchId);
            return account;
        }
    }
}
