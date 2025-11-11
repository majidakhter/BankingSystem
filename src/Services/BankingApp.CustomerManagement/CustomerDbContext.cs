using BankingAppDDD.CustomerManagement.Application.Customers.Models;
using BankingAppDDD.CustomerManagement.Core.Customers.Entities;
using BankingAppDDD.CustomerManagement.Infrastructure.Configurations;
using BankingAppDDD.Domains.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDDD.CustomerManagement
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CustomerDbContext : DbContext
    {
        private static readonly ILoggerFactory DebugLoggerFactory = new LoggerFactory(new[] { new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider() });
        private readonly IWebHostEnvironment? _env;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="env"></param>
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options, IWebHostEnvironment? env
            ) : base(options)
        {
            _env = env;
        }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<Customer> Customers { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_env != null && _env.IsDevelopment())
            {
                // used to print activity when debugging
                optionsBuilder.UseLoggerFactory(DebugLoggerFactory);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerEntityConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerTypeEntityConfiguration).Assembly);
            modelBuilder.Entity<CustomerType>().HasData(
            new List<CustomerType>(){
                    new CustomerType(1 , "RegularCustomer"),
                    new CustomerType( 2, "CorporateCustomer" ),
                    new CustomerType( 3, "VisitorCustomer" )
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

        }
    }
}
