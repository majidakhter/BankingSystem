using Autofac;
using Autofac.Extensions.DependencyInjection;
using BankingAppDDD.Common.Extension;
using BankingAppDDD.Common.Types;
using BankingAppDDD.UserManagement;
using BankingAppDDD.UserManagement.Application.Users.DomainEventHandlers;
using BankingAppDDD.MongoService.Application.Mongo;
using BankingAppDDD.UserManagement.Infrastructure.AutofacModules;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using BankingAppDDD.Common.Model;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.
builder.AddHostLogging();
services.AddWebHostInfrastructure(builder.Configuration, "CustomerManagementService");
services.Configure<PolyConfigSettings>(builder.Configuration.GetSection("PolyConfiguraionSettings"));
services.AddApiVersioning(ApiVersions.V2);
services.AddControllers();
var headers = new[] { "X-Operation", "X-Resource", "X-Total-Count" };
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();

// Register Token Service and Auth Handler
//services.AddHttpClient<KeycloakTokenService>();
//services.AddTransient<KeycloakAuthHandler>();

// Read settings from appsettings.json
var baseUrl = builder.Configuration["Keycloak:BaseUrl"] ?? "http://localhost:8080";
var clientid = builder.Configuration["Keycloak:ClientId"] ?? "customermanagementclient";
var username = builder.Configuration["Keycloak:UserName"] ?? "admin";
var password = builder.Configuration["Keycloak:Password"] ?? "admin";

var mongoConnStr = builder.Configuration["MongoDbSettings:MongoConnectionString"];
if (!string.IsNullOrEmpty(mongoConnStr))
{
    builder.Services.AddSingleton<MongoDB.Driver.IMongoClient>(sp => new MongoDB.Driver.MongoClient(mongoConnStr));
}
builder.Services.AddScoped<IUserMongoService, UserMongoService>();

services.AddAuthorization();
// Kiota client
var connectionString = builder.Configuration["DbContextSettings:ConnectionString"];
services.AddDbContext<UserDbContext>(opts => { opts.UseNpgsql(connectionString); });


services.AddSwaggerDocs();
services.AddHealthChecks()
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
    configure.SetKebabCaseEndpointNameFormatter();
    //var entryAssembly = Assembly.GetExecutingAssembly(); //this will be used if cosumer exist in webapi project itself now i moved to class library project so below code will work
    var entryAssembly = typeof(LoanApplicationCreatedEventHandler).Assembly;
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
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>((hostContext, container) =>
{
    container.RegisterModule(new ApplicationModule());
    container.RegisterModule(new InfrastructureModule(builder.Configuration));
});
var app = builder.Build();

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
app.UseRouting();
//app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("healthz");
app.MapHealthChecks("liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});
app.MapControllers();
app.Run();
