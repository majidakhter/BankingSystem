using BankingAppDDD.Common.Types;
using BankingAppDDD.PaymentProcessing.Application.ProcessingPayment.Consumers;
using BankingAppDDD.PaymentProcessing.Domain.Gateways;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// API Versioning
services.AddApiVersioning();

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
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

// App
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();