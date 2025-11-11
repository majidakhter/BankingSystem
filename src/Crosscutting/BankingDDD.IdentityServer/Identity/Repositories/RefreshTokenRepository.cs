

using BankingAppDDD.Identity.MongoModel;

namespace BankingAppDDD.Identity.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        public Task AddAsync(UserRefreshToken token)
        {
            throw new NotImplementedException();
        }

        public Task<UserRefreshToken> GetAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(UserRefreshToken token)
        {
            throw new NotImplementedException();
        }
    }
}
