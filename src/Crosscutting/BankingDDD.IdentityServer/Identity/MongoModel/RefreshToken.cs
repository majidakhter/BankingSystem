using BankingAppDDD.Common.Types;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Reflection.Emit;

namespace BankingAppDDD.Identity.MongoModel
{
    [BsonDiscriminator(RootClass = true)]
    public class UserRefreshToken : IIdentifiable
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get;  set; }
        public string? ReadableKey { get; set; }
        public string? UserId { get;  set; }
        public string? Token { get;  set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get;  set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? RevokedAt { get;  set; }
        public bool Revoked => RevokedAt.HasValue;

        public void Revoke()
        {
            if (Revoked)
            {
                //throw new DShopException(Codes.RefreshTokenAlreadyRevoked,
                  //  $"Refresh token: '{Id}' was already revoked at '{RevokedAt}'.");
            }
            RevokedAt = DateTime.UtcNow;
        }
       
    }
}
