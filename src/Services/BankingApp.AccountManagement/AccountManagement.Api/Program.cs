using Autofac;
using Autofac.Extensions.DependencyInjection;
using BankingApp.AccountManagement;
using BankingApp.AccountManagement.Infrastructure.AutofacModules;
using BankingAppDDD.AccountManagement.Application.Accounts.DomainEventHandlers.AccountCreated;
using BankingAppDDD.Common.Extension;
using BankingAppDDD.Common.Model;
using BankingAppDDD.Common.Types;
using BankingAppDDD.MongoService.Application.Mongo;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.
builder.AddHostLogging();
services.AddWebHostInfrastructure(builder.Configuration, "AccountManagementService");
services.Configure<PolyConfigSettings>(builder.Configuration.GetSection("PolyConfiguraionSettings"));
services.AddApiVersioning(ApiVersions.V2);
services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});
var headers = new[] { "X-Operation", "X-Resource", "X-Total-Count" };
services.AddEndpointsApiExplorer();
services.AddRedis(builder.Configuration);
services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddAuthorization();
var connectionString = builder.Configuration["DbContextSettings:ConnectionString"];
services.AddDbContext<AccountDbContext>(opts => { opts.UseNpgsql(connectionString); });
var mongoConnStr = builder.Configuration["MongoDbSettings:MongoConnectionString"];
if (!string.IsNullOrEmpty(mongoConnStr))
{
    builder.Services.AddSingleton<MongoDB.Driver.IMongoClient>(sp => new MongoDB.Driver.MongoClient(mongoConnStr));
}
services.AddScoped<IAccountMongoService, AccountMongoService>();
services.AddSwaggerDocs();
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("Application is running"))
#if (UseSqlServer)
    .AddSqlServer(builder.Configuration["Database:SqlConnectionString"]!);
#else
    .AddNpgSql(connectionString);
#endif
var rabbitMqHost = builder.Configuration["RabbitMqUrl:Host"] ?? throw new ArgumentNullException("RabbitMqUrl:Host section was not found");
var Username = builder.Configuration["RabbitMqUrl:Username"] ?? throw new ArgumentNullException("RabbitMqUrl:Username section was not found");
var Password = builder.Configuration["RabbitMqUrl:Password"] ?? throw new ArgumentNullException("RabbitMqUrl:Password section was not found");
services.AddMassTransit(configure =>
{
    //var entryAssembly = Assembly.GetExecutingAssembly(); //this will be used if cosumer exist in webapi project itself now i moved to class library project so below code will work
    var entryAssembly = typeof(AccountProvisionedDomainEventHandler).Assembly;
    configure.AddConsumers(entryAssembly);
    configure.SetKebabCaseEndpointNameFormatter();
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

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>((hostContext, container) =>
{
    container.RegisterModule(new ApplicationModule());
    container.RegisterModule(new InfrastructureModule(builder.Configuration));
});
var app = builder.Build();
string seedDataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SeedData");
if (!Directory.Exists(seedDataDirectory))
{
    seedDataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SeedData");
}
await DataSeeder.SeedDataAsync(app, seedDataDirectory);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

}
app.UseSwaggerDocs();

app.UseCors(options =>
{
    options.AllowAnyMethod()
           .AllowAnyHeader()
           .AllowAnyOrigin()
           .WithExposedHeaders("Content-Disposition");
});
//This is Required when we are doing ssl communication
//app.UseHttpsRedirection();
app.UseRouting();
//app.UseAuthentication();
//app.UseAccessTokenValidator();
app.UseAuthorization();
app.MapHealthChecks("healthz");
app.MapHealthChecks("liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});
app.MapControllers();
app.Run();
