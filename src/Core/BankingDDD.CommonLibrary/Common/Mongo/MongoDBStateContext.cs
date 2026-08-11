using BankingAppDDD.Common.Mongo.Interfaces.Collection;

namespace Common.Mongo
{
    public class MongoDBStateContext : IMongoDBStateContext
    {
        public string? CollectionName { get; set; }
        public TimeSpan TTLExpiration { get; set; }
        public string? ExpirationFieldName { get; set; }
    }
}
