using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using BankingAppDDD.Common.Mongo.Interfaces.Client;
using BankingAppDDD.Common.Mongo.Helper;
using BankingAppDDD.Common.Mongo.Interfaces.Operations;
using BankingAppDDD.Common.Mongo;
using BankingAppDDD.Identity.MongoModel;
using BankingAppDDD.Identity.Model;
using BankingAppDDD.Identity.Helper;
using BankingAppDDD.Common.Mongo.Interfaces.Collection;

namespace BankingAppDDD.Identity.Services
{
    public class MongoService : IMongoService
    {
        private readonly IDocument? _document;
        private readonly IConfiguration? _configuration;
        private readonly string? _dataVersion;
        private readonly double ExpireAfterDays;
        private readonly IMongoClient? _client;
        private readonly IMongoDatabase? dataBase;
        private readonly Collections collections;
        public MongoService(IConfiguration configuration, IMongoClient client, IOptions<Collections> mySettingsOptions)
        {
            collections = mySettingsOptions.Value;

            if (configuration != null)
            {
                _configuration = configuration;
                if (!string.IsNullOrEmpty(configuration.GetSection($"MongoDbSettings:MongoConnectionString").Value))
                {
                    _dataVersion = configuration.GetSection($"MongoDbSettings:DataVersion").Value;
                    ExpireAfterDays = Convert.ToDouble(configuration.GetSection($"DataBases:ExpireAfterDays").Value);
                    var conventions = new ConventionPack { new IgnoreExtraElementsConvention(true) };
                    ConventionRegistry.Register("Conventions", conventions, x => true);
                    _client = client;
                    var databaseName = configuration.GetValue<string>("MongoDbSettings:Database");
                    dataBase = _client.GetDatabase(databaseName);
                    IMongoDbContext context = new MongoDbContext(dataBase, mySettingsOptions);
                    _document = new Document(context);
                }


            }
        }
        public async Task<bool> SaveRefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                var key = DataHelper.GetRefreshTokenKey(request, _dataVersion);
                var readKey = DataHelper.GetSolutionRefreshTokenReadableKey(request, _dataVersion);
                var refreshToken = new UserRefreshToken
                {
                    Id = key,
                    ReadableKey = readKey,
                    UserId = request.UserId,
                    Token = request.Token,
                    CreatedAt = DateTime.Now,
                    RevokedAt = DateTime.Now,
                };

                IMongoDBStateContext statecontext = new MongoDBStateContext { CollectionName = collections.RefreshTokenCollection, ExpirationFieldName = "ModifiedDate", TTLExpiration = TimeSpan.FromDays(ExpireAfterDays) };
                var result = await _document.AddOneAsync(refreshToken, CancellationToken.None, statecontext);
                return result.Success;

            }
            catch (Exception ex)
            {
                throw new Exception($"SaveSolutionQuoteAndOfferOneAsync save failed in MongoDB for {request}", ex);
            }
        }



    }
}
