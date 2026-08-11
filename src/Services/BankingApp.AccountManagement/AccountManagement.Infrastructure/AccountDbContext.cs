
using BankingApp.AccountManagement.Infrastructure.Configurations;
using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.Domains.Accounts.Models;
using BankingAppDDD.Domains.Banks.Entities;
using BankingAppDDD.Domains.Branches.Entities;
using BankingAppDDD.Domains.CustomerAccounts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement
{
    public sealed class AccountDbContext : DbContext
    {
        private static readonly ILoggerFactory DebugLoggerFactory = new LoggerFactory(new[] { new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider() });
        private readonly IHostEnvironment? _env;

        public AccountDbContext(DbContextOptions<AccountDbContext> options, IHostEnvironment? env
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
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<FundTransferTransaction> FundTransferTransactions { get; set; }



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

            modelBuilder.HasSequence<int>("MyTransactionSequence").IncrementsBy(1).HasMin(-2000000).HasMax(2000100).StartsAt(1100001).IsCyclic();
            modelBuilder.Entity<Credit>().Property(pm => pm.TransactionNo).HasDefaultValueSql("nextval('\"MyTransactionSequence\"')");
            modelBuilder.Entity<Debit>().Property(pm => pm.TransactionNo).HasDefaultValueSql("nextval('\"MyTransactionSequence\"')");

            modelBuilder.Entity<UserAccount>().HasKey(c => c.UserId);

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
                if (aggregateBuild.Metadata.ClrType.Name == "CustomerAccount")
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
