using BankingAppDDD.Common.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;


namespace BankingAppDDD.Common.Extension
{
    public static class JwtExtensions
    {
        public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration is null)
                throw new ArgumentNullException(nameof(configuration));
            var baseUrl = configuration
            .GetValue<string>("Keycloak:BaseUrl")
            ?? throw new ArgumentNullException("Keycloak:BaseUrl section was not found");
            var realm = configuration
            .GetValue<string>("Keycloak:Realm")
            ?? throw new ArgumentNullException("Keycloak:Realm section was not found");
            var clientId = configuration
            .GetValue<string>("Keycloak:ClientId")
            ?? throw new ArgumentNullException("Keycloak:ClientId section was not found");
           
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"{baseUrl}/realms/{realm}"; // e.g., https://localhost:8080/realms/myrealm
                options.Audience = $"{clientId}"; // The Client ID of your confidential client
                options.RequireHttpsMetadata = false; // Set to false only for development with http
                options.SaveToken = true;
                options.IncludeErrorDetails = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidIssuer = $"{baseUrl}/realms/{realm}", //  http://localhost:8888/realms/bankaccount
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false,
                    RoleClaimType = "roles",
                    IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                    {
                        var client = new HttpClient();
                        var keyUri = $"{parameters.ValidIssuer}/protocol/openid-connect/certs";
                        var response = client.GetAsync(keyUri).Result;
                        var keys = new JsonWebKeySet(response.Content.ReadAsStringAsync().Result);
                        return keys.GetSigningKeys();
                    }
                };
            });
            return services;
        }

        public static IApplicationBuilder UseAccessTokenValidator(this IApplicationBuilder app)
          => app.UseMiddleware<AccessTokenValidatorMiddleware>();

    }
}
