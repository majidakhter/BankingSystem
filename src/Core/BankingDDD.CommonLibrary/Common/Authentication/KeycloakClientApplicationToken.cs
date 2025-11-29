using Common.Authentication;

namespace BankingAppDDD.Common.Authentication
{
    public class KeycloakClientApplicationToken(string keycloakBaseUrl, string realm, string clientId, string clientSecret) : IKeycloakClientApplicationToken
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _keycloakBaseUrl = keycloakBaseUrl;
        private readonly string _realm = realm;
        private readonly string _clientId = clientId;
        private readonly string _clientSecret = clientSecret;

        public async Task<string> GetApplicationTokenAsync()
        {
            var tokenEndpoint = $"{_keycloakBaseUrl}/realms/{_realm}/protocol/openid-connect/token";

            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", _clientId),
            new KeyValuePair<string, string>("client_secret", _clientSecret)
        });

            var response = await _httpClient.PostAsync(tokenEndpoint, content);
            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadAsStringAsync();
            // Parse the JSON response to extract the "access_token"
            // Example: using a JSON library like System.Text.Json or Newtonsoft.Json
            // var jsonDoc = JsonDocument.Parse(tokenResponse);
            // return jsonDoc.RootElement.GetProperty("access_token").GetString();
            return ExtractAccessTokenFromJson(tokenResponse); // Custom helper to extract token
        }

        // Helper method to extract access token (can be replaced with a proper JSON parser)
        private string ExtractAccessTokenFromJson(string jsonResponse)
        {
            var startIndex = jsonResponse.IndexOf("access_token\":\"") + "access_token\":\"".Length;
            var endIndex = jsonResponse.IndexOf("\"", startIndex);
            return jsonResponse.Substring(startIndex, endIndex - startIndex);
        }
    }
}
