using BankingApp.PaymentProcessing.Application.Command;
using BankingAppDDD.Common.Extension;
using BankingAppDDD.Common.Types;
using BankingAppDDD.PaymentProcessing.Application.ProcessingPayment.Consumers;
using BankingAppDDD.PaymentProcessing.Domain.Gateways;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// API Versioning & Logging
builder.AddHostLogging();
services.AddWebHostInfrastructure(builder.Configuration, "PaymentService");
services.AddApiVersioning(ApiVersions.V2);

// Register MediatR CQRS Pipeline
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RequestPhonePePaymentCommand).Assembly));

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerDocs();
services.AddHealthChecks();

// Payment Gateway Services & Resilience Setup
services.AddScoped<IPaymentGatewayService, RazorPayPaymentService>();
services.AddScoped<IPaymentGatewayService, PhonePePaymentService>();
services.AddScoped<IPaymentGatewayFactory, PaymentGatewayFactory>();

// MassTransit Configuration with RabbitMQ & Retries
var rabbitMqHost = builder.Configuration["RabbitMqUrl:Host"] ?? "localhost";
var username = builder.Configuration["RabbitMqUrl:Username"] ?? "guest";
var password = builder.Configuration["RabbitMqUrl:Password"] ?? "guest";

services.AddMassTransit(configure =>
{
    configure.AddConsumer<FundTransferRequestedConsumer>();
    configure.SetKebabCaseEndpointNameFormatter();
    configure.UsingRabbitMq((context, config) =>
    {
        config.Host(new Uri($"rabbitmq://{rabbitMqHost}"), h =>
        {
            h.Username(username);
            h.Password(password);
        });
        // Automatic message retry policy for transient broker/network issues
        config.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        config.ConfigureEndpoints(context);
    });
});
var corsOriginUrl = builder.Configuration["CorsOrigin"] ?? throw new ArgumentNullException("CorsOrigin section was not found");
var headers = new[] { "X-Operation", "X-Resource", "X-Total-Count" };
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
// App
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
app.UseAuthorization();
app.MapHealthChecks("healthz");
app.MapHealthChecks("liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});
app.MapControllers();

app.Run();