using System.Text.Json.Serialization;

namespace BankingAppDDD.KeyCloakClientLibrary.KeyCloakRestHelper
{
    public record TokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_in")] int ExpiresIn,
    [property: JsonPropertyName("refresh_token")] string RefreshToken,
    [property: JsonPropertyName("token_type")] string TokenType
);
}
