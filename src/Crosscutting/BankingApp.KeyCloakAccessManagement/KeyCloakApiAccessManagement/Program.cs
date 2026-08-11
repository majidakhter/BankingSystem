using BankingAppDDD.Common.Types;
using keycloak;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.
builder.AddHostLogging();
services.AddWebHostInfrastructure(builder.Configuration, "AccountManagementService");
services.AddApiVersioning(ApiVersions.V2);
services.AddControllers();
var headers = new[] { "X-Operation", "X-Resource", "X-Total-Count" };
services.AddEndpointsApiExplorer();
// Add services to the container.

builder.Services.AddControllers();
// Register HttpClient for fetching authentication tokens

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
