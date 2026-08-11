using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Domains.Abstractions.Models;
using System;

namespace BankingAppDDD.Applications.Abstractions.Shared.Commands
{
    public record CreateUserProfileCommand(
        UserIdentityData userdata,
        AddressData AddressData,
        Guid KeyCloakUserId,
        int accountTypeId,
        decimal amount,
        Guid? BranchId = null
    ) : Command;
}
