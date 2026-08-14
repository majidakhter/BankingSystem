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
using BankingAppDDD.UserManagement.Core.Users.Entities;
using BankingAppDDD.UserManagement.Application.Mongo.Helper;
using Common.Mongo;
using BankingAppDDD.MongoService.Mongo.Model;

namespace BankingAppDDD.MongoService.Application.Mongo
{
    public class UserMongoService : IUserMongoService
    {
        private readonly IDocument? _document;
        private readonly IConfiguration? _configuration;
        private readonly string? _dataVersion;
        private readonly double ExpireAfterDays;
        private readonly IMongoClient? _client;
        private readonly IMongoDatabase? dataBase;
        private readonly Collections? collections;

        public UserMongoService(IConfiguration configuration, IMongoClient? client = null, IOptions<Collections>? mySettingsOptions = null)
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

        public async Task<bool> SaveUserAsync(User request)
        {
            try
            {
                if (_document == null || collections == null) return false;
                Random r = new Random();
                int userversion = r.Next();
                var key = DataHelper.GetUserKey(request.Id, userversion, _dataVersion!);
                var readKey = DataHelper.GetUserReadableKey(request.Id, userversion, _dataVersion!);
                var userModel = new UserReadModelMapper
                {
                    CacheKey = key,
                    ReadableKey = readKey,
                    UserId = request.Id,
                    KeycloakUserId = request.KeyCloakUserId,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    BranchId = request.BranchId
                };


                IMongoDBStateContext statecontext = new MongoDBStateContext { CollectionName = "UserReadModel", ExpirationFieldName = "ModifiedDate", TTLExpiration = TimeSpan.FromDays(ExpireAfterDays) };
                var result = await _document.AddOneAsync(userModel,CancellationToken.None, statecontext);
                return result.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<AccountReadModelMapper?> GetAccountByUserIdAsync(Guid userId)
        {
            try
            {
                if (dataBase != null)
                {
                    var collection = dataBase.GetCollection<AccountReadModelMapper>("AccountReadModel");
                    var accountList = await collection.Find(a => a.UserId == userId || a.AccountId == userId).ToListAsync();
                    if (accountList != null && accountList.Count > 0)
                    {
                        var latestAcct = accountList.OrderByDescending(a => a.AccountBalance).FirstOrDefault();
                        if (latestAcct != null) return latestAcct;
                    }

                    var userCol = dataBase.GetCollection<UserReadModelMapper>("UserReadModel");
                    var user = await userCol.Find(a => a.UserId == userId || a.KeycloakUserId == userId).FirstOrDefaultAsync();
                    if (user != null)
                    {
                        accountList = await collection.Find(a => a.UserId == user.KeycloakUserId || a.UserId == user.UserId).ToListAsync();
                        if (accountList != null && accountList.Count > 0)
                        {
                            var latestAcct = accountList.OrderByDescending(a => a.AccountBalance).FirstOrDefault();
                            if (latestAcct != null) return latestAcct;
                        }
                    }
                }

                if (_document == null) return null;
                IMongoDBStateContext statecontext = new MongoDBStateContext { CollectionName = "AccountReadModel" };
                var account = await _document.GetOneAsync<AccountReadModelMapper>(a => a.UserId == userId || a.AccountId == userId, CancellationToken.None, statecontext);
                if (account != null) return account;

                IMongoDBStateContext userstatecontext = new MongoDBStateContext { CollectionName = "UserReadModel" };
                var userRead = await _document.GetOneAsync<UserReadModelMapper>(a => a.UserId == userId || a.KeycloakUserId == userId, CancellationToken.None, userstatecontext);
                if (userRead != null)
                {
                    account = await _document.GetOneAsync<AccountReadModelMapper>(a => a.UserId == userRead.KeycloakUserId || a.UserId == userRead.UserId, CancellationToken.None, statecontext);
                }
                return account;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<UserReadModelMapper?> GetUserByUserIdAsync(Guid userId)
        {
            try
            {
                if (_document == null) return null;
                IMongoDBStateContext userstatecontext = new MongoDBStateContext { CollectionName = "UserReadModel" };
                var userReadModel = await _document.GetOneAsync<UserReadModelMapper>(a => a.UserId == userId || a.KeycloakUserId == userId, CancellationToken.None, userstatecontext);
                return userReadModel;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
