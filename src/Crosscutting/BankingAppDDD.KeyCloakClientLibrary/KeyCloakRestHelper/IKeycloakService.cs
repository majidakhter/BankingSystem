

namespace BankingAppDDD.KeyCloakClientLibrary.KeyCloakRestHelper
{
    public interface IKeycloakService
    {
        Task<TokenResponse?> GetUserTokenAsync(string username, string password);
        Task<Guid> CreateUserAsync(UserCreationRequest request);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> AssignRoleToUserAsync(string userId, string roleName, bool isClientRole = false, string clientUniqueId = null);
        Task<bool> AssignClientRolesToUserAsync(string userId, string clientDbId, List<string> roleNames);
    }
}
