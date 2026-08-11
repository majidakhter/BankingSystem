using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BankingAppDDD.MongoService.Mongo.Model
{
    [BsonDiscriminator(RootClass = true)]
    public class AccountReadModelMapper
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid CacheKey { get; set; }

        public string? ReadableKey { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid AccountId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        public int AccountNo { get; set; }
        public int AccountTypeId { get; set; }
        public int AccountStatusId { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal AccountBalance { get; set; }
    }
}
