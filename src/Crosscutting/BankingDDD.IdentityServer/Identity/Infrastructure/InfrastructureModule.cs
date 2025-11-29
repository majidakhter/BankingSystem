using Autofac;
using BankingAppDDD.Common.Authentication;

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
            // Register AccessTokenValidatorMiddleware
            builder.RegisterType<AccessTokenValidatorMiddleware>().AsSelf();
           
        }
      
    }
}
