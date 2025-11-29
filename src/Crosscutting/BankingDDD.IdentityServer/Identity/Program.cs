using Autofac;
using Autofac.Extensions.DependencyInjection;
using BankingApp.Identity.Infrastructure.AutofacModules;
using BankingAppDDD.Common.Extension;
using BankingAppDDD.Common.Handlers;
using BankingAppDDD.Common.Mongo.Helper;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
builder.AddHostLogging();
services.AddWebHostInfrastructure(builder.Configuration, "IdentityService");
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);
services.AddHealthChecks();

services.AddAuthorization();
services.AddScoped<IAuthorizationHandler, RolesAuthorizationHandler>();
services.Configure<Collections>(builder.Configuration.GetSection("MongoDbSettings").GetSection("Collections"));
var Headers = new[] { "X-Operation", "X-Resource", "X-Total-Count" };

services.AddDependencyServices(builder.Configuration);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>((hostContext, container) =>
{
    container.RegisterModule(new InfrastructureModule());
});
services
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