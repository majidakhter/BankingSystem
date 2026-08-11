using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BankingAppDDD.KeyCloakClientLibrary.KeyCloakRestHelper
{
    public class KeycloakService : IKeycloakService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly string _baseUrl;
        private readonly string _realm;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public KeycloakService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;

            _baseUrl = _config["Keycloak:BaseUrl"]!.TrimEnd('/');
            _realm = _config["Keycloak:Realm"]!;
            _clientId = _config["Keycloak:ClientId"]!;
            _clientSecret = _config["Keycloak:ClientSecret"]!;
        }

        // 1. Get Token for a Specific User (Resource Owner Password Credentials Grant)
        public async Task<TokenResponse?> GetUserTokenAsync(string username, string password)
        {
            var url = $"{_baseUrl}/realms/{_realm}/protocol/openid-connect/token";

            var kvp = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "password"),
            new("client_id", _clientId),
            new("client_secret", _clientSecret),
            new("username", username),
            new("password", password)
        };

            var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(kvp));
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<TokenResponse>();
        }

        // Helper: Obtain Admin / Service Account Token to authenticate Admin REST API Requests
        private async Task<string> GetAdminTokenAsync()
        {
            var url = $"{_baseUrl}/realms/{_realm}/protocol/openid-connect/token";

            var kvp = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "client_credentials"),
            new("client_id", _clientId),
            new("client_secret", _clientSecret)
        };
            var kvp1 = new List<KeyValuePair<string, string>>
          {
                new KeyValuePair<string, string>("client_id", "admin-cli"),
                new KeyValuePair<string, string>("username", "admin"),
                new KeyValuePair<string, string>("password", "admin"),
                new KeyValuePair<string, string>("grant_type", "password")
            };
            var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(kvp));
            response.EnsureSuccessStatusCode();

            var tokenData = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return tokenData?.AccessToken ?? throw new Exception("Failed to fetch admin token.");
        }

        // 2. Create User via Admin REST API
        public async Task<Guid> CreateUserAsync(UserCreationRequest request)
        {
            var url = $"{_baseUrl}/admin/realms/{_realm}/users";
            var adminToken = await GetAdminTokenAsync();

            var newUser = new KeycloakUserDto
            {
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Enabled = true,
                Credentials = new List<KeycloakCredentialDto>
            {
                new() { Value = request.Password }
            }
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
            httpRequest.Content = JsonContent.Create(newUser);

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
            var uri = new Uri(response.Headers.Location!.ToString()).Segments.Last();
            return Guid.Parse(uri);
        }

        // 3. Delete User via Admin REST API
        public async Task<bool> DeleteUserAsync(string userId)
        {
            var url = $"{_baseUrl}/admin/realms/{_realm}/users/{userId}";
            var adminToken = await GetAdminTokenAsync();

            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, url);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await _httpClient.SendAsync(httpRequest);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AssignRoleToUserAsync(string userId, string roleName, bool isClientRole = true, string clientUniqueId = null)
        {
            // 1. Get Access Token using Service Account credentials
            string accessToken = await GetAdminTokenAsync();

            // 2. Fetch the target role's full definition object from Keycloak
             KeycloakRole roleRepresentation = !isClientRole
             ? await GetClientRoleDetailsAsync(accessToken, _clientId, roleName)
             : await GetRealmRoleDetailsAsync(accessToken, roleName);
            //KeycloakRole roleRepresentation = await GetClientRoleDetailsAsync(accessToken, _clientId, roleName);
            // 3. Post the role payload to Keycloak to complete assignment
            await AssignRolePayloadAsync(accessToken, userId, roleRepresentation, isClientRole, clientUniqueId);
            return true;
        }

        private async Task<KeycloakRole> GetRealmRoleDetailsAsync(string token, string roleName)
        {
            var url = $"{_baseUrl}/admin/realms/{_realm}/roles/{Uri.EscapeDataString(roleName)}";
            return await GetRoleFromUrlAsync(token, url);
        }

        private async Task<KeycloakRole> GetClientRoleDetailsAsync(string token, string clientUniqueId, string roleName)
        {
            // Note: clientUniqueId is the internal UUID of the client (not the human-readable Client ID)
            var url = $"{_baseUrl}/admin/realms/{_realm}/clients/{clientUniqueId}/roles/{Uri.EscapeDataString(roleName)}";
            return await GetRoleFromUrlAsync(token, url);
        }

        private async Task<KeycloakRole> GetRoleFromUrlAsync(string token, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<KeycloakRole>();
        }

        private async Task AssignRolePayloadAsync(string token, string userId, KeycloakRole role, bool isClientRole, string clientUniqueId)
        {
            // The endpoint path changes based on whether it is a Realm or Client level role
            string targetUrl = isClientRole
                ? $"{_baseUrl}/admin/realms/{_realm}/users/{userId}/role-mappings/clients/{clientUniqueId}"
                : $"{_baseUrl}/admin/realms/{_realm}/users/{userId}/role-mappings/realm"; //

            var request = new HttpRequestMessage(HttpMethod.Post, targetUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Keycloak expects a JSON Array of Role objects, even if you are only assigning one role
            request.Content = JsonContent.Create(new List<KeycloakRole> { role });

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode(); // Expects 204 No Content upon success
        }

        public async Task<bool> AssignClientRolesToUserAsync(string userId, string clientDbId, List<string> roleNames)
        {
            // 1. Get Access Token using Service Account
            string accessToken = await GetAdminTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // 2. Fetch the target roles from Keycloak to get their UUIDs
            var rolesToAssign = new List<object>();
            foreach (var roleName in roleNames)
            {
                var roleDetails = await GetClientRoleDetailsAsync(clientDbId, roleName);
                if (roleDetails != null)
                {
                    rolesToAssign.Add(roleDetails);
                }
            }

            if (rolesToAssign.Count == 0) return false;

            // 3. POST the role representations to the user's client-level role-mappings endpoint
            string url = $"{_baseUrl}/{_realm}/users/{userId}/role-mappings/clients/{clientDbId}";
            var jsonContent = new StringContent(JsonSerializer.Serialize(rolesToAssign), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, jsonContent);
            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to assign roles: {response.StatusCode} - {error}");
            }
            return true;
        }
        private async Task<object> GetClientRoleDetailsAsync(string clientDbId, string roleName)
        {
            string url = $"{_baseUrl}/{_realm}/clients/{clientDbId}/roles/{roleName}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) return null;

            string json = await response.Content.ReadAsStringAsync();
            // Returns the partial RoleRepresentation containing 'id' and 'name'
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }
    }
}
