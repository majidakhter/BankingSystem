using BankingAppDDD.MongoService.Mongo.Model;
using BankingAppDDD.UserManagement.Core.Users.Entities;

namespace BankingAppDDD.MongoService.Application.Mongo
{
    public interface IUserMongoService
    {
        Task<bool> SaveUserAsync(User request);
        Task<AccountReadModelMapper?> GetAccountByUserIdAsync(Guid userId);
        Task<UserReadModelMapper?> GetUserByUserIdAsync(Guid userId);
    }
}
