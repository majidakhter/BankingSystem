using BankingApp.AccountManagement.Core.Accounts.Entities;
using BankingApp.AccountManagement.Core.Banks.Entities;
using BankingApp.AccountManagement.Core.Branches.Entities;
using BankingApp.AccountManagement.Core.Customers.Entities;
using BankingApp.AccountManagement.Infrastructure.Configurations;
using BankingAppDDD.Domains.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.AccountManagement
{
    public sealed class AccountDbContext : DbContext
    {
        private static readonly ILoggerFactory DebugLoggerFactory = new LoggerFactory(new[] { new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider() });
        private readonly IWebHostEnvironment? _env;

        public AccountDbContext(DbContextOptions<AccountDbContext> options, IWebHostEnvironment? env
            ) : base(options)
        {
            _env = env;
        }

        public DbSet<Bank> Banks { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountStatus> AccountStatuses { get; set; }
        public DbSet<Debit> Debits { get; set; }
        public DbSet<Credit> Credits { get; set; }
        public DbSet<BeneficiaryGroup> Beneficaries { get; set; }
        public DbSet<Customer> Customers { get; set; }


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
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankEntityConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BranchEntityConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountEntityConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountStatusEntityConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountTypeEntityConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BeneficiaryEntityConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CreditEntityConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DebitEntityConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerEntityTypeConfiguration).Assembly);

            modelBuilder.HasSequence<int>("MySimpleSequence").IncrementsBy(1).HasMin(-2000000).HasMax(2000000).StartsAt(1000001).IsCyclic();
            modelBuilder.Entity<Account>().Property(pm => pm.AccountNo).HasDefaultValueSql("nextval('\"MySimpleSequence\"')");


            modelBuilder.Entity<AccountStatus>().HasData(
             new List<AccountStatus>(){
                    new AccountStatus(1 , "Opened"),
                    new AccountStatus( 2, "Locked" ),
                    new AccountStatus( 3, "Closed" )
                    }.ToArray());

            modelBuilder.Entity<AccountType>().HasData(
             new List<AccountType>(){
                    new AccountType(1 , "Savings"),
                    new AccountType( 2, "Current" ),
                    new AccountType( 3, "Loan" ),
                    new AccountType( 4, "PPF" )
             }.ToArray());


            var aggregateTypes = modelBuilder.Model
                                             .GetEntityTypes()
                                             .Select(e => e.ClrType)
                                             .Where(e => !e.IsAbstract && e.IsAssignableTo(typeof(EntityBase)));

            foreach (var type in aggregateTypes)
            {

                var aggregateBuild = modelBuilder.Entity(type);
                aggregateBuild.Ignore(nameof(EntityBase.DomainEvents));
                if (aggregateBuild.Metadata.ClrType.Name == "Customer")
                {
                    aggregateBuild.Ignore(nameof(EntityBase.Id));
                }
                else
                {
                    aggregateBuild.Property(nameof(EntityBase.Id)).ValueGeneratedNever();
                }


            }
        }
    }
}
