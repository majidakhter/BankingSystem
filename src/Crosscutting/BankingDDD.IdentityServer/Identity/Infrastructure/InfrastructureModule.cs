using Autofac;
using BankingAppDDD.Common.Authentication;
using BankingAppDDD.Identity.Services;

namespace BankingApp.Identity.Infrastructure.AutofacModules
{
    public sealed class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterType<AccessTokenService>() // Replace AccessTokenService with your actual implementation
                   .As<IAccessTokenService>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<JwtHandler>() // Replace with your actual implementation class
             .As<IJwtHandler>()
             .InstancePerLifetimeScope();
            builder.RegisterType<KeycloakAuthService>() // Replace AccessTokenService with your actual implementation
                   .As<IKeycloakAuthService>()
                   .InstancePerLifetimeScope();
            // Register AccessTokenValidatorMiddleware
            builder.RegisterType<AccessTokenValidatorMiddleware>().AsSelf();
           
        }
      
    }
}
