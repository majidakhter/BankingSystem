using Autofac;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Common.Authentication;
using BankingAppDDD.CustomerManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BankingAppDDD.CustomerManagement.Infrastructure.AutofacModules
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class InfrastructureModule : Module
    {
        private readonly DbContextOptions<CustomerDbContext> _options;
        private readonly IConfiguration Configuration;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public InfrastructureModule(IConfiguration configuration) : this(CreateDbOptions(configuration), configuration)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="configuration"></param>
        public InfrastructureModule(DbContextOptions<CustomerDbContext> options, IConfiguration configuration)
        {
            Configuration = configuration;
            _options = options;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            // builder.RegisterInstance(Options.Create(DatabaseSettings.Create(Configuration)));
            builder.RegisterType<CustomerDbContext>()
                .AsSelf()
                .InstancePerRequest()
                .InstancePerLifetimeScope()
                .WithParameter(new NamedParameter("options", _options));

            builder.RegisterType<UnitOfWork>()
                .AsImplementedInterfaces()
                .InstancePerRequest()
                .InstancePerLifetimeScope();
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterType<AccessTokenService>() // Replace AccessTokenService with your actual implementation
                   .As<IAccessTokenService>()
                   .InstancePerLifetimeScope();
            // Register AccessTokenValidatorMiddleware
            builder.RegisterType<AccessTokenValidatorMiddleware>().AsSelf();
            builder.RegisterGeneric(typeof(Repository<>))
                .As(typeof(IRepository<>));
            builder.RegisterGeneric(typeof(Logger<>))
                .As(typeof(ILogger<>));
            //builder.RegisterType<NotificationsService>()
            // .AsImplementedInterfaces()
            // .SingleInstance();
        }

        private static DbContextOptions<CustomerDbContext> CreateDbOptions(IConfiguration configuration)
        {
            var connectionString = configuration["DbContextSettings:ConnectionString"];
            return new DbContextOptionsBuilder<CustomerDbContext>()
              //.UseNpgsql("Host=127.0.0.1;Database=CBSAccount;Port=5432;User ID=Majid;Password=Alm1ghty;Pooling=true;")
              .UseNpgsql(connectionString)
              .Options;
        }
    }
}
