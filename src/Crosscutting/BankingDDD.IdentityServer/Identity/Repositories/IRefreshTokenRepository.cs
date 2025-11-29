using BankingAppDDD.Identity.MongoModel;

namespace BankingAppDDD.Identity.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<UserRefreshToken> GetAsync(string token);
        Task AddAsync(UserRefreshToken token);
        Task UpdateAsync(UserRefreshToken token);
    }
}
