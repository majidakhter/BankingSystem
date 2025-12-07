using Autofac;
using Autofac.Extensions.DependencyInjection;
using BankingAppDDD.Common.Extension;
using BankingAppDDD.Common.Handlers;
using BankingAppDDD.Common.Types;
using BankingAppDDD.CustomerManagement;
using BankingAppDDD.CustomerManagement.Infrastructure.AutofacModules;
using BankingDDD.ServiceClient.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.
builder.AddHostLogging();
services.AddWebHostInfrastructure(builder.Configuration, "CustomerManagementService");
// API Versioning
services.AddApiVersioning(ApiVersions.V2);
services.AddControllers();
var headers = new[] { "X-Operation", "X-Resource", "X-Total-Count" };
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);
services.AddHealthChecks();
services.AddHttpContextAccessor();
services.AddAuthorization();
services.AddScoped<IAuthorizationHandler, RolesAuthorizationHandler>();
// Kiota client
services.AddApiGatewayClient(builder.Configuration);
var connectionString = builder.Configuration["DbContextSettings:ConnectionString"];
services.AddDbContext<CustomerDbContext>(opts => { opts.UseNpgsql(connectionString); });

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
    configure.SetKebabCaseEndpointNameFormatter();
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
services
   .AddCors(options =>
   {
       options.AddPolicy("AllowOrigin",
                    builder => builder.WithOrigins("http://localhost:5157") //url here need to change from http to https if we are doing ssl communication
                             .AllowAnyHeader()
                             .AllowAnyMethod()
                         .AllowCredentials()
                         .WithExposedHeaders(headers));
   });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseSwaggerDocs();

//This is Required when we are doing ssl communication
//app.UseHttpsRedirection();
app.UseCors("AllowOrigin");
app.UseRouting();
app.UseAuthentication();
app.UseAccessTokenValidator();
app.UseAuthorization();

app.MapControllers();
app.UseHealthChecks();

app.Run();
