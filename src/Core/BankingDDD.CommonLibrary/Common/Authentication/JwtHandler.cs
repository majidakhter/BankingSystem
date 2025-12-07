using DnsClient.Internal;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BankingAppDDD.Common.Authentication
{
    public class JwtHandler : IJwtHandler
    {

        //private readonly JwtOptions _options;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtHandler> _logger;
       // private readonly List<User> _users = new()
       // {
           // new("admin", "ADm1n","Administrator",["writers.read"]),
           // new("user01", "u$3r01","User",["writers.noread"])
       // };
        public JwtHandler(HttpClient httpClient, IConfiguration configuration, ILogger<JwtHandler> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<JsonWebToken> GetToken(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("User name claim can not be empty.", nameof(userName));
            }

            var tokenEndpoint = $"{_configuration["Keycloak:BaseUrl"]}/realms/{_configuration["Keycloak:Realm"]}/protocol/openid-connect/token";
            _logger.LogInformation("token Endpoint: {@endpoint}", tokenEndpoint);
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
            _logger.LogInformation("Created token response: {@tokenresponse}", responseText);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResult = JsonSerializer.Deserialize<JsonWebToken>(responseContent);
                _logger.LogInformation("Deserialized token: {@deserialisedtoken}", tokenResult);
                return tokenResult!;
            }

            throw new Exception("failed");
        }

    }
}