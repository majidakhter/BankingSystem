
using BankingAppDDD.Common.Cache;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using StackExchange.Redis;
using System.Text.Json.Serialization;

namespace BankingAppDDD.Common.Extension
{
    public static class HostDiExtensions
    {
       
        public static IServiceCollection AddWebHostInfrastructure(this IServiceCollection services, IConfiguration configuration, string serviceName)
        {
            services
                .AddRedis(configuration)
                .AddHostOpenTelemetry(serviceName);

            return services;
        }

        public static void AddHostLogging(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));
        }
        private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetConnectionString("Redis")!;
           
                ConfigurationOptions option = new ConfigurationOptions
                {
                    AbortOnConnectFail = false,
                    EndPoints = { "redis:6379" }
                };
            
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(option);

            services
                .AddSingleton<ICacheService, CacheService>()
                .AddSingleton(connectionMultiplexer)
                .AddStackExchangeRedisCache(options =>
                {
                    
                    options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer);
                });

            return services;
        }
        private static IServiceCollection AddHostOpenTelemetry(this IServiceCollection services, string serviceName)
        {
            services
                .AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(serviceName))
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRedisInstrumentation()
                        .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
                        .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources")
                        .AddSource("MailKit");

                    tracing.AddOtlpExporter();
                });

            return services;
        }
    }
}
