using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BankingAppDDD.MongoService.Mongo.Model
{
    [BsonDiscriminator(RootClass = true)]
    public class BankReadModelMapper
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid CacheKey { get; set; }
        public string? ReadableKey { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid BankId { get;  set; }
        public string Name { get;  set; }
        public DateTime DateAdded { get;  set; }
    }
}
