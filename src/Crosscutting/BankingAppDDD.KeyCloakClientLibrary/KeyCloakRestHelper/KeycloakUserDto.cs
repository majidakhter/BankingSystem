using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BankingAppDDD.KeyCloakClientLibrary.KeyCloakRestHelper
{
    public class KeycloakUserDto
    {
        [JsonPropertyName("username")] public string Username { get; set; } = string.Empty;
        [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
        [JsonPropertyName("firstName")] public string FirstName { get; set; } = string.Empty;
        [JsonPropertyName("lastName")] public string LastName { get; set; } = string.Empty;
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        [JsonPropertyName("credentials")] public List<KeycloakCredentialDto> Credentials { get; set; } = new();
    }
}
