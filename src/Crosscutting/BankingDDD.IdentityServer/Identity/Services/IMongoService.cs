using BankingAppDDD.Identity.Model;

namespace BankingAppDDD.Identity.Services
{
    public interface IMongoService
    {
        Task<bool> SaveRefreshTokenAsync(RefreshTokenRequest request);
    }
}
