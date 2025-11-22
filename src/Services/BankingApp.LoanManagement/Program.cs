using Autofac;
using Autofac.Extensions.DependencyInjection;
using BankingApp.LoanManagement;
using BankingApp.LoanManagement.Infrastructure.AutofacModules;
using BankingAppDDD.Common.Extension;
using BankingAppDDD.Common.Handlers;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.AddHostLogging();
builder.Services.AddWebHostInfrastructure(builder.Configuration, "CreditService");
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerDocs();
builder.Services.AddJwt();
builder.Services.AddAuthorization();
builder.Services.AddScoped<IAuthorizationHandler, RolesAuthorizationHandler>();
builder.Services.AddHttpContextAccessor();
var connectionString = builder.Configuration["DbContextSettings:ConnectionString"];
builder.Services.AddDbContext<CreditMgmtDbContext>(opts => { opts.UseNpgsql(connectionString); });
var Headers = new[] { "X-Operation", "X-Resource", "X-Total-Count" };
builder.Services.AddHttpClient();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>((hostContext, container) =>
{
    container.RegisterModule(new ApplicationModule());
    container.RegisterModule(new InfrastructureModule(builder.Configuration));
});
builder.Services.AddMassTransit(configure =>
{
    configure.SetKebabCaseEndpointNameFormatter();
    configure.UsingRabbitMq((context, config) =>
    {
        config.Host(new Uri("rabbitmq://rabbitmq"), h =>
        {
            h.Username("guest");
            h.Password("guest");

        });

    });
});
builder.Services
   .AddCors(options =>
   {
       options.AddPolicy("AllowOrigin",
                    builder => builder.WithOrigins("http://localhost:5273")
                             .AllowAnyHeader()
                             .AllowAnyMethod()
                         .AllowCredentials()
                         .WithExposedHeaders(Headers));
   });
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
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
app.UseCors("AllowOrigin");
//app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAccessTokenValidator();
app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks();
app.Run();
 