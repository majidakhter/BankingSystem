using Autofac;
using Autofac.Extensions.DependencyInjection;
using BankingApp.AccountManagement;
using BankingApp.AccountManagement.Core.Accounts.Entities;
using BankingApp.AccountManagement.Core.Banks.Entities;
using BankingApp.AccountManagement.Core.Branches.Entities;
using BankingApp.AccountManagement.Core.Customers.Entities;
using BankingApp.AccountManagement.Infrastructure.AutofacModules;
using BankingAppDDD.Common.Extension;
using BankingAppDDD.Common.Handlers;
using BankingAppDDD.Common.Types;
using BankingDDD.ServiceClient.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Polly;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.
builder.AddHostLogging();
services.AddWebHostInfrastructure(builder.Configuration, "AccountManagementService");
services.AddApiVersioning(ApiVersions.V2);
services.AddControllers();
var headers = new[] { "X-Operation", "X-Resource", "X-Total-Count" };
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);
services.AddHealthChecks();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddAuthorization();
services.AddScoped<IAuthorizationHandler, RolesAuthorizationHandler>();
// Kiota client
services.AddApiGatewayClient(builder.Configuration);
var connectionString = builder.Configuration["DbContextSettings:ConnectionString"];
services.AddDbContext<AccountDbContext>(opts => { opts.UseNpgsql(connectionString); });
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>((hostContext, container) =>
{
    container.RegisterModule(new ApplicationModule());
    container.RegisterModule(new InfrastructureModule(builder.Configuration));
});
var rabbitMqHost = builder.Configuration["RabbitMqUrl:Host"] ?? throw new ArgumentNullException("RabbitMqUrl:Host section was not found");
var Username = builder.Configuration["RabbitMqUrl:Username"] ?? throw new ArgumentNullException("RabbitMqUrl:Username section was not found");
var Password = builder.Configuration["RabbitMqUrl:Password"] ?? throw new ArgumentNullException("RabbitMqUrl:Password section was not found");
services.AddMassTransit(configure =>
{
    var entryAssembly = Assembly.GetExecutingAssembly();

    configure.AddConsumers(entryAssembly);
    configure.UsingRabbitMq((context, config) =>
    {
        config.Host(new Uri($"rabbitmq://{rabbitMqHost}"), h =>
        {
            h.Username($"{Username}");
            h.Password($"{Password}");
        });
        config.ConfigureEndpoints(context);
    });
   
});
var corsOriginUrl = builder.Configuration["CorsOrigin"] ?? throw new ArgumentNullException("CorsOrigin section was not found");
services
   .AddCors(options =>
   {
       options.AddPolicy("AllowOrigin",
                    builder => builder.WithOrigins(corsOriginUrl) //url here need to change from http to https if we are doing ssl communication
                             .AllowAnyHeader()
                             .AllowAnyMethod()
                         .AllowCredentials()
                         .WithExposedHeaders(headers));
   });

services.AddHsts(options =>
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
