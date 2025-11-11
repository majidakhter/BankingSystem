using BankingAppDDD.Common.Authentication;
using BankingAppDDD.Common.Types;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDDD.Common.Extension
{
    public static class JwtExtensions
    {
        private static readonly string SectionName = "jwt";
        public static void AddJwt(this IServiceCollection services)
        {
            IConfiguration? configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>();
            }
            // var section = configuration!.GetSection(SectionName);
            // var options = configuration.GetOptions<JwtOptions>(SectionName);
            // services.Configure<JwtOptions>(section);
            //services.AddSingleton(options);

            //services.AddSingleton<IJwtHandler, JwtHandler>();
            services.AddTransient<IAccessTokenService, AccessTokenService>();
            services.AddTransient<AccessTokenValidatorMiddleware>();
            /*services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidIssuer = $"{configuration["Keycloak:BaseUrl"]}/realms/{configuration["Keycloak:Realm"]}",
                      ValidateAudience = true,
                      ValidAudience = "account",
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
                  options.RequireHttpsMetadata = false; // Only in develop environment
                  options.SaveToken = true;
                  options.IncludeErrorDetails = true;
              }
              )
              .Services
                .AddHttpClient()
                .AddSwaggerGen()
                .AddControllers();*/
            var appSettings = configuration!.Get<AppSettings>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = $"{appSettings!.Keycloak.BaseUrl}/realms/{appSettings!.Keycloak.Realm}"; // e.g., https://localhost:8080/realms/myrealm
            options.Audience = $"{appSettings!.Keycloak.ClientId}"; // The Client ID of your confidential client
            options.RequireHttpsMetadata = false; // Set to false only for development with http
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                RoleClaimType = "roles",
            };
        });
        }

        public static IApplicationBuilder UseAccessTokenValidator(this IApplicationBuilder app)
          => app.UseMiddleware<AccessTokenValidatorMiddleware>();

        public static long ToTimestamp(this DateTime dateTime)
        {
            var centuryBegin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var expectedDate = dateTime.Subtract(new TimeSpan(centuryBegin.Ticks));

            return expectedDate.Ticks / 10000;
        }
    }
}
