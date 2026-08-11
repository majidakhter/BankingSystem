using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BankingAppDDD.MongoService.Mongo.Model
{
    [BsonDiscriminator(RootClass = true)]
    public class BranchReadModelMapper
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid CacheKey { get; set; }
        public string? ReadableKey { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid BranchId { get;  set; }
        public Guid BankId { get;  set; }
        public string Name { get;  set; }
        public string BranchCode { get;  set; }
        public string IfscCode { get;  set; }
        public int MICRCode { get;  set; }
        public string PhoneNumber { get;  set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
    }
}
