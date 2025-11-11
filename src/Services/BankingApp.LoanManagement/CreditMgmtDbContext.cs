using BankingApp.LoanManagement.Application.LoanApplicationModels;
using BankingApp.LoanManagement.Core.DebtInfos.Entities;
using BankingApp.LoanManagement.Core.LoanApplicationsEntities;
using BankingApp.LoanManagement.Infrastructure.Configurations;
using BankingAppDDD.Domains.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;
using SharpCompress.Common;

namespace BankingApp.LoanManagement
{
    public sealed class CreditMgmtDbContext : DbContext
    {
        private static readonly ILoggerFactory DebugLoggerFactory = new LoggerFactory(new[] { new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider() });
        private readonly IWebHostEnvironment? _env;
        public CreditMgmtDbContext(DbContextOptions<CreditMgmtDbContext> options,
            IWebHostEnvironment? env) : base(options)
        {
            _env = env;
        }

        public DbSet<LoanApplication> LoanApplications { get; set; }

        //public DbSet<LoanApplicationStatus> LoanApplicationStatuses { get; set; }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<DebtorInfo> DebtorInfos { get; set; }
        public DbSet<LoanType> LoanTypes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_env != null && _env.IsDevelopment())
            {
                // used to print activity when debugging
                optionsBuilder.UseLoggerFactory(DebugLoggerFactory);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LoanApplicationEntityConfiguration).Assembly);
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(LoanApplicationStatusEntityConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OperatorConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DebtorInfoEntityConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LoanTypeConfiguration).Assembly);

            //modelBuilder.Entity<LoanApplicationStatus>().HasData(
             //new List<LoanApplicationStatus>(){
                   // new LoanApplicationStatus(1 , "None"),
                   // new LoanApplicationStatus( 2, "New" ),
                   // new LoanApplicationStatus( 3, "Accepted" ),
                    // new LoanApplicationStatus( 4, "Rejected" )
                   // }.ToArray());

            modelBuilder.Entity<LoanType>().HasData(
            new List<LoanType>(){
                    new LoanType(1 , "Mortgage"),
                    new LoanType( 2, "Education" ),
                    new LoanType( 3, "Home" ),
                    new LoanType( 4, "Car" ),
                    new LoanType( 5, "Personal" ),
                    new LoanType( 6, "Gold" )
                   }.ToArray());

            var aggregateTypes = modelBuilder.Model
                                             .GetEntityTypes()
                                             .Select(e => e.ClrType)
                                             .Where(e => !e.IsAbstract && e.IsAssignableTo(typeof(EntityBase)));

            foreach (var type in aggregateTypes)
            {
                var aggregateBuild = modelBuilder.Entity(type);
                aggregateBuild.Ignore(nameof(EntityBase.DomainEvents));
                aggregateBuild.Property(nameof(EntityBase.Id)).ValueGeneratedNever();
            }

            /*var valueTypes = modelBuilder.Model
                                            .GetEntityTypes()
                                            .Select(e => e.ClrType)
                                            .Where(e => !e.IsAbstract && e.IsAssignableTo(typeof(ValueObject)));
            foreach (var type in valueTypes)
            {

                var valueBuild = modelBuilder.Entity(type);
                if (valueBuild.Metadata.ClrType.Name == "Operator")
                {
                    if (!valueBuild.Metadata.GetKeys().Any())
                    {
                        // If no primary key exists, add an 'Id' property and configure it as the primary key
                        valueBuild.Property<int>("Id")
                              .ValueGeneratedOnAdd(); // Configures as an identity column

                        valueBuild.HasKey("Id"); // Sets 'Id' as the primary key
                    }
                }
                else
                {
                    valueBuild.Ignore(nameof(ValueObject));
                }*/

            }
            

        }
    
}
