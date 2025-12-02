using Autofac;
using Autofac.Extensions.DependencyInjection;
using BankingApp.LoanManagement;
using BankingApp.LoanManagement.Infrastructure.AutofacModules;
using BankingAppDDD.Common.Extension;
using BankingAppDDD.Common.Handlers;
using BankingAppDDD.Common.Types;
using BankingDDD.ServiceClient.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
builder.AddHostLogging();
services.AddWebHostInfrastructure(builder.Configuration, "CreditService");
// Add services to the container.
services.AddApiVersioning(ApiVersions.V2);
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);
services.AddHealthChecks();

services.AddAuthorization();
services.AddScoped<IAuthorizationHandler, RolesAuthorizationHandler>();
services.AddApiGatewayClient(builder.Configuration);

var connectionString = builder.Configuration["DbContextSettings:ConnectionString"];
services.AddDbContext<CreditMgmtDbContext>(opts => { opts.UseNpgsql(connectionString); });
var Headers = new[] { "X-Operation", "X-Resource", "X-Total-Count" };

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
    configure.UsingRabbitMq((context, config) =>
    {
        config.Host(new Uri($"rabbitmq://{rabbitMqHost}"), h =>
        {
            h.Username($"{Username}");
            h.Password($"{Password}");
        });

    });
});
services
   .AddCors(options =>
   {
       options.AddPolicy("AllowOrigin",
                    builder => builder.WithOrigins("http://localhost:5273")
                             .AllowAnyHeader()
                             .AllowAnyMethod()
                         .AllowCredentials()
                         .WithExposedHeaders(Headers));
   });
services.AddHsts(options =>
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

app.UseCors("AllowOrigin");
//app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAccessTokenValidator();
app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks();
app.Run();
 