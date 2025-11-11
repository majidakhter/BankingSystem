using Autofac;
using BankingApp.LoanManagement.Core.LoanApplicationsEntities;
using BankingApp.LoanManagement.Infrastructure.Abstraction;
using BankingApp.LoanManagement.Infrastructure.Repositories;
using BankingApp.LoanManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.LoanManagement.Infrastructure.AutofacModules
{
    public sealed class InfrastructureModule : Module
    {
        private readonly DbContextOptions<CreditMgmtDbContext> _options;
        private readonly IConfiguration Configuration;

        public InfrastructureModule(IConfiguration configuration) : this(CreateDbOptions(configuration), configuration)
        {

        }

        public InfrastructureModule(DbContextOptions<CreditMgmtDbContext> options, IConfiguration configuration)
        {
            Configuration = configuration;
            _options = options;
        }

        protected override void Load(ContainerBuilder builder)
        {
           // builder.RegisterInstance(Options.Create(DatabaseSettings.Create(Configuration)));
            builder.RegisterType<CreditMgmtDbContext>()
                .AsSelf()
                .InstancePerRequest()
                .InstancePerLifetimeScope()
                .WithParameter(new NamedParameter("options", _options));

            builder.RegisterType<UnitOfWork>()
                .AsImplementedInterfaces()
                .InstancePerRequest()
                .InstancePerLifetimeScope();

            //builder.RegisterGeneric(typeof(Repository<>))
                //.As(typeof(IRepository<>));
            builder.RegisterGeneric(typeof(Repository<>))
               .As(typeof(ILoanRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<Repository<LoanApplication>>()
              .As<ILoanRepository<LoanApplication>>()
              .InstancePerLifetimeScope();
            builder.RegisterType<Repository<Operator>>()
              .As<ILoanRepository<Operator>>()
              .InstancePerLifetimeScope();
            builder.RegisterType<DebtorRegistry>()
              .As<IDebtorRegistry>()
              .InstancePerLifetimeScope();

            //builder.RegisterType<NotificationsService>()
            // .AsImplementedInterfaces()
            // .SingleInstance();
        }

        private static DbContextOptions<CreditMgmtDbContext> CreateDbOptions(IConfiguration configuration)
        {
            var connectionString = configuration["DbContextSettings:ConnectionString"];
            return new DbContextOptionsBuilder<CreditMgmtDbContext>()
              //.UseNpgsql("Host=127.0.0.1;Database=CBSAccount;Port=5432;User ID=Majid;Password=Alm1ghty;Pooling=true;")
              .UseNpgsql(connectionString)
              .Options;
        }
    }
}
