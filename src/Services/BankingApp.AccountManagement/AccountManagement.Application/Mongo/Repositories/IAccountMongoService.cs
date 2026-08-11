using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.Domains.Banks.Entities;
using BankingAppDDD.Domains.Branches.Entities;
using BankingAppDDD.MongoService.Mongo.Model;

namespace BankingAppDDD.MongoService.Application.Mongo
{
    public interface IAccountMongoService
    {
        Task<bool> SaveBankDetailAsync(Bank request);
        Task<bool> SaveBranchDetailAsync(Branch request);
        Task<bool> SaveAccountDetailAsync(Account request);
        Task<bool> SaveTransferTransactionAsync(FundTransferTransaction transaction);
        Task<UserReadModelMapper?> GetUserByIdAsync(Guid userId);
        Task<bool> SaveCountriesAsync(List<CountryReadModel> countries);
        Task<bool> SaveStatesAsync(List<StateReadModel> states);
        Task<List<CountryReadModel>> GetCountriesAsync();
        Task<List<StateReadModel>> GetStatesAsync();
    }
}
