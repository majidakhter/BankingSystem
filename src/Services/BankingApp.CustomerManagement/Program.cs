using Autofac;
using Autofac.Extensions.DependencyInjection;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using BankingAppDDD.Common.Extension;
using BankingAppDDD.Common.Handlers;
using BankingAppDDD.CustomerManagement.Infrastructure.AutofacModules;
using BankingAppDDD.CustomerManagement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddHostLogging();
builder.Services.AddWebHostInfrastructure(builder.Configuration, "CustomerManagementService");
builder.Services.AddControllers();
var headers = new[] { "X-Operation", "X-Resource", "X-Total-Count" };
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddSwaggerDocs();
builder.Services.AddJwt();
builder.Services.AddAuthorization();
builder.Services.AddScoped<IAuthorizationHandler, RolesAuthorizationHandler>();
var connectionString = builder.Configuration["DbContextSettings:ConnectionString"];
builder.Services.AddDbContext<CustomerDbContext>(opts => { opts.UseNpgsql(connectionString); });
builder.Services.AddHttpClient();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>((hostContext, container) =>
{
    container.RegisterModule(new ApplicationModule());
    container.RegisterModule(new InfrastructureModule(builder.Configuration));
});
builder.Services.AddMassTransit(configure =>
{
    configure.SetKebabCaseEndpointNameFormatter();
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

var app = builder.Build();

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
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.UseHealthChecks();
app.Run();
