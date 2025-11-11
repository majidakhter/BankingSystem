using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace BankingAppDDD.Identity.Model
{
    [DataContract]
    public class RefreshTokenRequest
    {
        [DataMember]
        public string? UserId { get; set; }

        [DataMember]
        public string? Token { get; set; }

        [DataMember]
        public int TokenVersion { get; set; }

    }
}
