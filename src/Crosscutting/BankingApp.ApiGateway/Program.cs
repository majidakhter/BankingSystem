using BankingAppDDD.Common.Authentication;
using BankingAppDDD.Common.Extension;
using BankingAppDDD.Common.Handlers;
using BankingAppDDD.Common.Mongo.Helper;
using BankingAppDDD.Common.Types;
using Koalesce.Core.Extensions;
using Koalesce.OpenAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

// Load merged ocelot.json
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("Ocelot/ocelot.json", optional: false, reloadOnChange: true)
    .AddOcelot(
        folder: "Ocelot",
        env: builder.Environment,
        mergeTo: MergeOcelotJson.ToFile,
        primaryConfigFile: "Ocelot/ocelot.json",
        reloadOnChange: true
    )
    .AddEnvironmentVariables();

// Add services to the container.
services.AddApiVersioning(ApiVersions.V2);
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddHealthChecks();
services.AddJwt(builder.Configuration);
services.AddSwaggerGen();
services.AddSwaggerDocs();
services.AddRedis(builder.Configuration);
services.AddTransient<IAccessTokenService, AccessTokenService>();
services.AddTransient<AccessTokenValidatorMiddleware>();
services.AddScoped<IAuthorizationHandler, RolesAuthorizationHandler>();
//services.Configure<Collections>(builder.Configuration.GetSection("MongoDbSettings").GetSection("Collections"));

services.AddOcelot(builder.Configuration);
// Register Koalesce
services.AddKoalesce(builder.Configuration)
    .ForOpenAPI();
var headers = new[] { "X-Operation", "X-Resource", "X-Total-Count" };

const string corsPolicy = "AllowOrigin";

services
   .AddCors(options =>
   {
       options.AddPolicy(corsPolicy,
                    builder => builder.WithOrigins("http://localhost:5000") //url here need to change from http to https if we are doing ssl communication
                             .AllowAnyHeader()
                             .AllowAnyMethod()
                         .AllowCredentials()
                         .SetIsOriginAllowed(x => true)
                         .WithExposedHeaders(headers));
   });

var app = builder.Build();

app.UseCors(corsPolicy);
app.UseWebSockets();
app.UseRouting();
app.UseAuthentication();
app.UseAccessTokenValidator();
app.UseAuthorization();
app.UseHealthChecks();

// Enable Koalesce before Swagger Middleware
app.UseKoalesce();

// Enable Swagger
app.UseSwagger();
app.MapControllers();
KoalesceOptions koalesceOptions;
using (var scope = app.Services.CreateScope())
{
    koalesceOptions = scope.ServiceProvider
        .GetRequiredService<IOptions<KoalesceOptions>>().Value;
    // Enable Swagger UI
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(koalesceOptions.MergedOpenApiPath, koalesceOptions.Title);
        c.OAuthClientId(app.Configuration.GetValue<string>("Keycloak:ClientId")); // The client ID configured in Keycloak
        c.OAuthClientSecret(app.Configuration.GetValue<string>("Keycloak:ClientSecret"));
        c.OAuthScopes("openid profile");
        c.OAuthAppName("BankingApp ApiGateway - Keycloak Integration");
        c.OAuthUsePkce();
    });
}

app.UseOcelot().Wait();
app.Run();
