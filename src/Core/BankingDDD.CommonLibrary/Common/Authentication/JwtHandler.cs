using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace BankingAppDDD.Common.Authentication
{
    public class JwtHandler : IJwtHandler
    {

        private readonly JwtOptions _options;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public JwtHandler(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

        }

        public async Task<JsonWebToken> GetToken(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("User name claim can not be empty.", nameof(userName));
            }

            var tokenEndpoint = $"{_configuration["Keycloak:BaseUrl"]}/realms/{_configuration["Keycloak:Realm"]}/protocol/openid-connect/token";

            var requestBody = new Dictionary<string, string>
            {
               { "grant_type", "password" },
               { "client_id", _configuration["Keycloak:ClientId"]! },
               { "client_secret", _configuration["Keycloak:ClientSecret"]! },
               { "username", userName },
               { "password", password }
             };
            // http://localhost:8080/realms/fauly-realm/protocol/openid-connect/token
            var content = new FormUrlEncodedContent(requestBody);

            var response = await _httpClient.PostAsync(tokenEndpoint, content);
            var responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResult = JsonSerializer.Deserialize<JsonWebToken>(responseContent);

                return tokenResult!;
            }

            throw new Exception("failed");
        }

    }
}