using Autofac;
using Autofac.Extensions.DependencyInjection;
using BankingAppDDD.Common.Extension;
using BankingAppDDD.Common.Handlers;
using BankingAppDDD.Common.Mongo.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Newtonsoft.Json.Linq;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Sockets;
using System.Runtime;
using BankingApp.Identity.Infrastructure.AutofacModules;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.AddHostLogging();
builder.Services.AddWebHostInfrastructure(builder.Configuration, "IdentityService");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddSwaggerDocs();
//builder.Services.AddTransient<IAccessTokenService, AccessTokenService>();

builder.Services.AddJwt();
builder.Services.AddAuthorization();
builder.Services.AddScoped<IAuthorizationHandler, RolesAuthorizationHandler>();
builder.Services.AddHttpContextAccessor();
ConfigurationManager Configuration = builder.Configuration;
builder.Services.Configure<Collections>(builder.Configuration.GetSection("MongoDbSettings").GetSection("Collections"));
var Headers = new[] { "X-Operation", "X-Resource", "X-Total-Count" };
builder.Services.AddHttpClient();
//builder.Services.AddScoped<IKeycloakAuthService, KeycloakAuthService>();

builder.Services.AddDependencyServices(Configuration);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>((hostContext, container) =>
{
    container.RegisterModule(new InfrastructureModule());
});
builder.Services
   .AddCors(options =>
    {
        options.AddPolicy("AllowOrigin",
                     builder => builder.WithOrigins("http://localhost:5263")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                          .AllowCredentials()
                          .WithExposedHeaders(Headers));
    });
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

}
app.UseSwaggerDocs();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/docs/v1/swagger.json", "v1");
    options.OAuthClientId(app.Configuration.GetValue<string>("Keycloak:ClientId"));
    options.OAuthClientSecret(app.Configuration.GetValue<string>("Keycloak:ClientSecret"));
    options.OAuthScopes("openid profile email");
    options.OAuthUsePkce();


});
//app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAccessTokenValidator();
app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks();
app.Run();