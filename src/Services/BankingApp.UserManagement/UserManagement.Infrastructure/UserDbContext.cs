using BankingAppDDD.UserManagement.Core.Users.Entities;
using BankingAppDDD.UserManagement.Infrastructure.Configurations;
using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Users.Models;
using BankingAppDDD.UserManagement.Core.Users.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BankingAppDDD.UserManagement
{
    /// <summary>
    /// User bounded context DbContext containing User entities and CQRS Read Models
    /// </summary>
    public sealed class UserDbContext : DbContext
    {
        private static readonly ILoggerFactory DebugLoggerFactory = new LoggerFactory(new[] { new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider() });
        private readonly IHostEnvironment? _env;

        public UserDbContext(DbContextOptions<UserDbContext> options, IHostEnvironment? env
            ) : base(options)
        {
            _env = env;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserAccountReadModel> UserAccountReadModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_env != null && _env.IsDevelopment())
            {
                optionsBuilder.UseLoggerFactory(DebugLoggerFactory);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserTypeEntityConfiguration).Assembly);
            modelBuilder.Entity<UserType>().HasData(
            new List<UserType>(){
                    new UserType(1 , "RegularCustomer"),
                    new UserType( 2, "CorporateCustomer" ),
                    new UserType( 3, "VisitorCustomer" )
                   }.ToArray());

            modelBuilder.Entity<UserAccountReadModel>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.ToTable("UserAccountReadModels");
            });

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
        }
    }
}
