using Autofac;
using Autofac.Core;
using BankingAppDDD.Common.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
