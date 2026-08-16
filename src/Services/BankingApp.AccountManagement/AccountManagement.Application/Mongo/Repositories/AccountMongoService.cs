using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using BankingAppDDD.Common.Mongo.Interfaces.Client;
using BankingAppDDD.Common.Mongo.Helper;
using BankingAppDDD.Common.Mongo.Interfaces.Operations;
using BankingAppDDD.Common.Mongo;
using BankingAppDDD.Common.Mongo.Interfaces.Collection;
using Microsoft.Extensions.Configuration;
using Common.Mongo;
using BankingAppDDD.MongoService.Mongo.Model;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.AccountManagement.Application.Mongo.Helper;
using BankingAppDDD.Domains.Banks.Entities;
using BankingAppDDD.Domains.Branches.Entities;

namespace BankingAppDDD.MongoService.Application.Mongo
{
    public class AccountMongoService : IAccountMongoService
    {
        private readonly IDocument? _document;
        private readonly IConfiguration? _configuration;
        private readonly string? _dataVersion;
        private readonly double ExpireAfterDays;
        private readonly IMongoClient? _client;
        private readonly IMongoDatabase? dataBase;
        private readonly Collections? collections;

        public AccountMongoService(IConfiguration configuration, IMongoClient? client = null, IOptions<Collections>? mySettingsOptions = null)
        {
            if (mySettingsOptions != null)
                collections = mySettingsOptions.Value;

            if (configuration != null)
            {
                _configuration = configuration;
                if (!string.IsNullOrEmpty(configuration.GetSection($"MongoDbSettings:MongoConnectionString").Value))
                {
                    _dataVersion = configuration.GetSection($"MongoDbSettings:DataVersion").Value;
                    var expireVal = configuration.GetSection($"DataBases:ExpireAfterDays").Value;
                    if (!string.IsNullOrEmpty(expireVal))
                    {
                        ExpireAfterDays = Convert.ToDouble(expireVal);
                    }

                    try
                    {
                        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    }
                    catch (BsonSerializationException)
                    {
                        // Serializer registered already
                    }

                    var conventions = new ConventionPack { new IgnoreExtraElementsConvention(true) };
                    ConventionRegistry.Register("Conventions", conventions, x => true);
                    _client = client ?? new MongoClient(configuration.GetSection($"MongoDbSettings:MongoConnectionString").Value);
                    var databaseName = configuration.GetValue<string>("MongoDbSettings:Database");
                    if (!string.IsNullOrEmpty(databaseName))
                    {
                        dataBase = _client.GetDatabase(databaseName);
                        var options = mySettingsOptions ?? Options.Create(new Collections
                        {
                            RefreshTokenCollection = "RefreshTokenCollection",
                            UserReadModel = "UserReadModel",
                            AccountReadModel = "AccountReadModel"
                        });
                        IMongoDbContext context = new MongoDbContext(dataBase, options);
                        _document = new Document(context);
                    }
                }
            }
        }

        public async Task<bool> SaveBankDetailAsync(Bank request)
        {
            try
            {
                if (_document == null) return false;
                Random r = new Random();
                int bankversion = r.Next();
                var key = DataHelper.GetBankKey(request.Id, bankversion, _dataVersion ?? "V1");
                var readKey = DataHelper.GetBankReadableKey(request.Id, bankversion, _dataVersion ?? "V1");
                var accountReadModel = new BankReadModelMapper
                {
                    CacheKey = key,
                    ReadableKey = readKey,
                    BankId = request.Id,
                    Name = request.Name,
                    DateAdded = request.DateAdded ?? DateTime.UtcNow
                };

                IMongoDBStateContext statecontext = new MongoDBStateContext { CollectionName = "BankReadModel", ExpirationFieldName = "ModifiedDate", TTLExpiration = TimeSpan.FromDays(ExpireAfterDays > 0 ? ExpireAfterDays : 365) };
                var result = await _document.AddOneAsync(accountReadModel, CancellationToken.None, statecontext);
                return result.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving bank detail to Mongo: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SaveBranchDetailAsync(Branch request)
        {
            try
            {
                if (_document == null) return false;
                Random r = new Random();
                int branchversion = r.Next();
                var key = DataHelper.GetBranchKey(request.Id, branchversion, _dataVersion ?? "V1");
                var readKey = DataHelper.GetBranchReadableKey(request.Id, branchversion, _dataVersion ?? "V1");
                var accountReadModel = new BranchReadModelMapper
                {
                    CacheKey = key,
                    ReadableKey = readKey,
                    BranchId = request.Id,
                    BankId = request.BankId,
                    BranchCode = request.BranchCode,
                    Name = request.Name,
                    IfscCode = request.IfscCode,
                    MICRCode = request.MICRCode,
                    PhoneNumber = request.PhoneNumber.Value,
                    Street = request.BranchAddress.Street,
                    City = request.BranchAddress.City,
                    State = request.BranchAddress.State,
                    Country = request.BranchAddress.Country,
                    ZipCode = request.BranchAddress.ZipCode
                };

                IMongoDBStateContext statecontext = new MongoDBStateContext { CollectionName = "BranchReadModel", ExpirationFieldName = "ModifiedDate", TTLExpiration = TimeSpan.FromDays(ExpireAfterDays > 0 ? ExpireAfterDays : 365) };
                var result = await _document.AddOneAsync(accountReadModel, CancellationToken.None, statecontext);
                return result.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving account detail to Mongo: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> SaveAccountDetailAsync(Account request)
        {
            try
            {
                if (_document == null) return false;

                IMongoDBStateContext statecontext = new MongoDBStateContext { CollectionName = "AccountReadModel", ExpirationFieldName = "ModifiedDate", TTLExpiration = TimeSpan.FromDays(ExpireAfterDays > 0 ? ExpireAfterDays : 365) };

                // Preserve existing CacheKey (_id) if document already exists to comply with Mongo's immutable _id constraint
                AccountReadModelMapper? existingDoc = null;
                try
                {
                    existingDoc = await _document.GetOneAsync<AccountReadModelMapper>(a => a.AccountId == request.Id, CancellationToken.None, statecontext);
                }
                catch { }

                var cacheKey = existingDoc != null ? existingDoc.CacheKey : request.Id;
                var readKey = existingDoc != null ? existingDoc.ReadableKey : DataHelper.GetAccountReadableKey(request.Id, 1, _dataVersion ?? "V1");

                var accountReadModel = new AccountReadModelMapper
                {
                    CacheKey = cacheKey,
                    ReadableKey = readKey,
                    AccountId = request.Id,
                    UserId = request.KeycloakUserId,
                    AccountNo = request.AccountNo,
                    AccountTypeId = request.AccountTypeId,
                    AccountStatusId = request.AccountStatusId,
                    AccountBalance = request.GetCurrentBalance().Value
                };

                var result = await _document.ReplaceOneAsync(accountReadModel, a => a.AccountId == request.Id, new ReplaceOptions { IsUpsert = true }, CancellationToken.None, statecontext);
                return result != null && result.IsAcknowledged;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving account detail to Mongo: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SaveTransferTransactionAsync(FundTransferTransaction transaction)
        {
            try
            {
                if (_document != null)
                {
                    IMongoDBStateContext statecontext = new MongoDBStateContext { CollectionName = "TransferTransactions", ExpirationFieldName = "CreatedAt", TTLExpiration = TimeSpan.FromDays(ExpireAfterDays > 0 ? ExpireAfterDays : 365) };
                    var result = await _document.ReplaceOneAsync(transaction, t => t.TransactionId == transaction.TransactionId, new ReplaceOptions { IsUpsert = true }, CancellationToken.None, statecontext);
                    return result != null && result.IsAcknowledged;
                }

                if (dataBase != null)
                {
                    var collection = dataBase.GetCollection<FundTransferTransaction>("TransferTransactions");
                    var result = await collection.ReplaceOneAsync(
                        t => t.TransactionId == transaction.TransactionId,
                        transaction,
                        new ReplaceOptions { IsUpsert = true });
                    return result.IsAcknowledged;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving transfer transaction to Mongo: {ex.Message}");
                return false;
            }
        }

        public async Task<UserReadModelMapper?> GetUserByIdAsync(Guid userId)
        {
            try
            {
                if (_document == null) return null;
                IMongoDBStateContext userstatecontext = new MongoDBStateContext { CollectionName = "UserReadModel" };
                var user = await _document.GetOneAsync<UserReadModelMapper>(a => a.UserId == userId || a.KeycloakUserId == userId, CancellationToken.None, userstatecontext);
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> SaveCountriesAsync(List<CountryReadModel> countries)
        {
            try
            {
                if (dataBase == null) return false;
                var collection = dataBase.GetCollection<CountryReadModel>("CountryReadModel");
                var existing = await collection.Find(_ => true).CountDocumentsAsync();
                if (existing == 0)
                {
                    await collection.InsertManyAsync(countries);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving countries to Mongo: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SaveStatesAsync(List<StateReadModel> states)
        {
            try
            {
                if (dataBase == null) return false;
                var collection = dataBase.GetCollection<StateReadModel>("StateReadModel");
                var existing = await collection.Find(_ => true).CountDocumentsAsync();
                if (existing == 0)
                {
                    await collection.InsertManyAsync(states);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving states to Mongo: {ex.Message}");
                return false;
            }
        }

        public async Task<List<CountryReadModel>> GetCountriesAsync()
        {
            try
            {
                if (dataBase == null) return new List<CountryReadModel>();
                var collection = dataBase.GetCollection<CountryReadModel>("CountryReadModel");
                return await collection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching countries from Mongo: {ex.Message}");
                return new List<CountryReadModel>();
            }
        }

        public async Task<List<StateReadModel>> GetStatesAsync()
        {
            try
            {
                if (dataBase == null) return new List<StateReadModel>();
                var collection = dataBase.GetCollection<StateReadModel>("StateReadModel");
                return await collection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching states from Mongo: {ex.Message}");
                return new List<StateReadModel>();
            }
        }
    }
}


