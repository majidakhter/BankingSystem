using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using BankingApp.AccountManagement;
using BankingApp.AccountManagement.Core.Accounts.Entities;
using BankingApp.AccountManagement.Core.Banks.Entities;
using BankingApp.AccountManagement.Core.Branches.Entities;
using BankingApp.AccountManagement.Core.Customers.Entities;
using BankingApp.AccountManagement.Infrastructure.AutofacModules;
using BankingAppDDD.Common.Handlers;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using BankingAppDDD.Common.Extension;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddHostLogging();
builder.Services.AddWebHostInfrastructure(builder.Configuration, "AccountManagementService");
builder.Services.AddControllers();
var headers = new[] { "X-Operation", "X-Resource", "X-Total-Count" };
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerDocs();
builder.Services.AddJwt();
builder.Services.AddAuthorization();
builder.Services.AddScoped<IAuthorizationHandler, RolesAuthorizationHandler>();
builder.Services.AddHttpContextAccessor();
var connectionString = builder.Configuration["DbContextSettings:ConnectionString"];
builder.Services.AddDbContext<AccountDbContext>(opts => { opts.UseNpgsql(connectionString); });
builder.Services.AddHttpClient();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>((hostContext, container) =>
{
    container.RegisterModule(new ApplicationModule());
    container.RegisterModule(new InfrastructureModule(builder.Configuration));
});
builder.Services.AddMassTransit(configure =>
{
    var entryAssembly = Assembly.GetExecutingAssembly();

    configure.AddConsumers(entryAssembly);
    configure.UsingRabbitMq((context, config) =>
    {
        config.Host(new Uri("rabbitmq://rabbitmq"), h =>
        {
            h.Username("guest");
            h.Password("guest");
            
        });
        config.ConfigureEndpoints(context);
    });
});
builder.Services
   .AddCors(options =>
   {
       options.AddPolicy("AllowOrigin",
                    builder => builder.WithOrigins("http://localhost:5210") //url here need to change from http to https if we are doing ssl communication
                             .AllowAnyHeader()
                             .AllowAnyMethod()
                         .AllowCredentials()
                         .WithExposedHeaders(headers));
   });

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});


var app = builder.Build();
string seedDataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SeedData");

var fileTypeMap = new Dictionary<string, Type>
{
    { "Bank.json", typeof(Bank) },
    { "Branches.json", typeof(Branch) },
    { "Accounts.json", typeof(Account) },
    { "BankCustomers.json", typeof(Customer) },
};
Dictionary<string, List<object>> allData = JsonSeedDataLoader.LoadMultipleSeedData(seedDataDirectory, fileTypeMap);
//app.Seed(allData).Run();

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
//This is Required when we are doing ssl communication
//app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAccessTokenValidator();
app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks();
app.Run();
