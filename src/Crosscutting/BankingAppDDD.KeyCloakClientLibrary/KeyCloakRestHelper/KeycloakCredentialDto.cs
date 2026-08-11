using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BankingAppDDD.KeyCloakClientLibrary.KeyCloakRestHelper
{
    public class KeycloakCredentialDto
    {
        [JsonPropertyName("type")] public string Type { get; set; } = "password";
        [JsonPropertyName("value")] public string Value { get; set; } = string.Empty;
        [JsonPropertyName("temporary")] public bool Temporary { get; set; } = false;
    }
}
