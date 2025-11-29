using System.Text.Json.Serialization;

namespace BankingAppDDD.Common.Authentication
{
    public class JsonWebToken
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("expires_in")]
        public long Expires { get; set; }

        [JsonPropertyName("preferred_username")]
        public string? Id { get; set; }

        [JsonPropertyName("roles")]
        public List<string>? Role { get; set; }

        [JsonPropertyName("claims")]
        public IDictionary<string, string>? Claims { get; set; }
    }
}