using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BankingAppDDD.MongoService.Mongo.Model
{
    [BsonDiscriminator(RootClass = true)]
    public class UserReadModelMapper
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid CacheKey { get; set; }

        public string? ReadableKey { get; set; }
         
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }
        public Guid KeycloakUserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid? BranchId { get; set; }
    }
}
