using Autofac;
using BankingApp.AccountManagement.Core.Accounts.Entities;
using BankingApp.AccountManagement.Core.Customers.Entities;
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Common.Authentication;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.AccountManagement.Infrastructure.AutofacModules
{
    public sealed class InfrastructureModule : Module
    {
        private readonly DbContextOptions<AccountDbContext> _options;
        private readonly IConfiguration Configuration;

        public InfrastructureModule(IConfiguration configuration) : this(CreateDbOptions(configuration), configuration)
        {

        }

        public InfrastructureModule(DbContextOptions<AccountDbContext> options, IConfiguration configuration)
        {
            Configuration = configuration;
            _options = options;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // builder.RegisterInstance(Options.Create(DatabaseSettings.Create(Configuration)));
            builder.RegisterType<AccountDbContext>()
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
            builder.RegisterGeneric(typeof(AccountRepository<>))
                .As(typeof(IAccountRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<AccountRepository<Account>>()
              .As<IAccountRepository<Account>>()
              .InstancePerLifetimeScope();
            // builder.RegisterType<AccountRepository<Account>>().As<IAccountNonGenericRepo>().InstancePerLifetimeScope();
            //builder.RegisterType<NotificationsService>()
            // .AsImplementedInterfaces()
            // .SingleInstance();
        }

        private static DbContextOptions<AccountDbContext> CreateDbOptions(IConfiguration configuration)
        {
            var connectionString = configuration["DbContextSettings:ConnectionString"];
            return new DbContextOptionsBuilder<AccountDbContext>()
              //.UseNpgsql("Host=127.0.0.1;Database=CBSAccount;Port=5432;User ID=Majid;Password=Alm1ghty;Pooling=true;")
              .UseNpgsql(connectionString)
              .Options;
        }
    }
}
