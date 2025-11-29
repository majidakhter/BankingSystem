using BankingAppDDD.Common.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace BankingAppDDD.Common.Extension
{
    public static class Extensions
    {

        public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
        {
            SwaggerOptions options;
            IConfiguration? configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {

                configuration = serviceProvider.GetService<IConfiguration>();
                services.Configure<SwaggerOptions>(configuration!.GetSection("swagger"));
                options = configuration.GetOptions<SwaggerOptions>("swagger");
            }

            if (!options.Enabled)
            {
                return services;
            }
            var appSettings = configuration!.Get<AppSettings>();
            var authurl = $"{appSettings!.Keycloak.BaseUrl}/realms/{appSettings!.Keycloak.Realm}/protocol/openid-connect/auth";
            var keycloaktokenUrl = $"{appSettings!.Keycloak.BaseUrl}/realms/{appSettings!.Keycloak.Realm}/protocol/openid-connect/token";
            return services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(options.Name, new OpenApiInfo { Title = options.Title, Version = options.Version });
                c.DescribeAllParametersInCamelCase();
                c.CustomSchemaIds(x => x.FullName);
                if (options.IncludeSecurity)
                {
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Scheme = "Bearer",
                        Flows = new OpenApiOAuthFlows
                        {
                            AuthorizationCode = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri(authurl),
                                TokenUrl = new Uri(keycloaktokenUrl),
                                Scopes = new Dictionary<string, string>
                                {
                                    { "openid", "openid" },
                                    { "profile", "profile" },
                                }
                            }
                        },

                    });
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                     {
                        {
                          new OpenApiSecurityScheme
                          {
                           Reference = new OpenApiReference
                            {
                            Type = ReferenceType.SecurityScheme,
                             Id = "Bearer"
                             },
                           Scheme = "oauth2",
                           Name = "Bearer",
                           In = ParameterLocation.Header,

                          },
                         new[] { "openid", "profile"}
                        }
                     });
                    // c.OperationFilter<AuthorizeOperationFilter>();
                }

            });
        }

        public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder builder)
        {
            var configuration = builder.ApplicationServices.GetService<IConfiguration>();
            if (configuration is null)
            {
                throw new InvalidOperationException("IConfiguration service is not registered in the application services.");
            }
            var options = configuration.GetOptions<SwaggerOptions>("swagger");
            if (!options.Enabled)
            {
                return builder;
            }
            var appSettings = configuration!.Get<AppSettings>();
            var routePrefix = string.IsNullOrWhiteSpace(options.RoutePrefix) ? "swagger" : options.RoutePrefix;

            builder.UseStaticFiles()
                .UseSwagger(c => c.RouteTemplate = routePrefix + "/{documentName}/swagger.json");
            return options.ReDocEnabled
                ? builder.UseReDoc(c =>
                {
                    c.RoutePrefix = routePrefix;
                    c.SpecUrl = $"{options.Name}/swagger.json";
                })
                : builder.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"/{routePrefix}/{options.Name}/swagger.json", options.Title);
                    c.RoutePrefix = routePrefix;
                    c.OAuthClientId(appSettings!.Keycloak.ClientId); // The client ID configured in Keycloak
                    c.OAuthClientSecret(appSettings!.Keycloak.ClientSecret);
                    c.OAuthScopes("openid profile");
                    c.OAuthAppName("Identity Service API - Keycloak Integration");
                    c.OAuthUsePkce();
                });
        }
    }
}

// Replace the GetOptions extension method to use Activator.CreateInstance instead of new()
// This allows instantiation of types with required members (C# 11+)
public static class Extensions
{
    public static TModel GetOptions<TModel>(this IConfiguration configuration, string section) where TModel : class
    {
        var model = Activator.CreateInstance(typeof(TModel)) as TModel;
        if (model == null)
            throw new InvalidOperationException($"Could not create an instance of type {typeof(TModel).FullName}.");
        configuration.GetSection(section).Bind(model);

        return model;
    }
}
