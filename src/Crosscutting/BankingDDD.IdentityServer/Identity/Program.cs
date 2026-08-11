using Autofac;
using Autofac.Extensions.DependencyInjection;
using BankingApp.Identity.Infrastructure.AutofacModules;
using BankingAppDDD.Common.Extension;
using BankingAppDDD.Common.Types;
using BankingAppDDD.Identity.Infrastructure.AutofacModules;
using BankingAppDDD.KeyCloakClientLibrary.KeyCloakRestHelper;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
builder.AddHostLogging();
services.AddWebHostInfrastructure(builder.Configuration, "Identity Service");
services.AddApiVersioning(ApiVersions.V2);
services.AddControllers(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});
services.AddEndpointsApiExplorer();
services.AddHttpContextAccessor();
services.AddHealthChecks();
builder.Services.AddHttpClient<IKeycloakService, KeycloakService>();
services.AddAuthorization();

var rabbitMqHost = builder.Configuration["RabbitMqUrl:Host"] ?? "localhost";
var rabbitMqUsername = builder.Configuration["RabbitMqUrl:Username"] ?? "guest";
var rabbitMqPassword = builder.Configuration["RabbitMqUrl:Password"] ?? "guest";

services.AddMassTransit(configure =>
{
    configure.SetKebabCaseEndpointNameFormatter();
    configure.UsingRabbitMq((context, config) =>
    {
        config.Host(new Uri($"rabbitmq://{rabbitMqHost}"), h =>
        {
            h.Username(rabbitMqUsername);
            h.Password(rabbitMqPassword);
        });
        config.ConfigureEndpoints(context);
    });
});

var Headers = new[] { "X-Operation", "X-Resource", "X-Total-Count" };
services.AddSwaggerDocs();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>((hostContext, container) =>
{
    container.RegisterModule(new ApplicationModule());
    container.RegisterModule(new InfrastructureModule());
});

var corsOriginUrl = builder.Configuration["CorsOrigin"] ?? throw new ArgumentNullException("CorsOrigin section was not found");
services
   .AddCors(options =>
    {
        options.AddPolicy("AllowOrigin",
                     builder => builder.WithOrigins(corsOriginUrl)
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

app.UseCors(options =>
{
    options.AllowAnyMethod()
           .AllowAnyHeader()
           .AllowAnyOrigin()
           .WithExposedHeaders("Content-Disposition");
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks();
app.Run();