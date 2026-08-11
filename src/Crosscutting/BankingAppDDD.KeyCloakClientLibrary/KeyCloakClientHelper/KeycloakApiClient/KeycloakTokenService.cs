using Microsoft.Extensions.Configuration;

namespace BankingAppDDD.KeyCloakClientLibrary.KeyCloakClientHelper.KeycloakApiClient
{
    public class KeycloakTokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public KeycloakTokenService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> GetAdminTokenAsync()
        {
            var section = _config.GetSection("Keycloak");
            var tokenEndpoint = $"{section["BaseUrl"]}/realms/master/protocol/openid-connect/token";

            var dict = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", section["AdminClientId"] },
            { "client_secret", section["AdminClientSecret"] }
        };

            var response = await _httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(dict));
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var tokenData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(content);
            return tokenData.access_token;
        }
    }
}
