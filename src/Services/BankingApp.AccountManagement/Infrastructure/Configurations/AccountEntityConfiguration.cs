
using BankingApp.AccountManagement.Core.Accounts.Entities;
using BankingApp.AccountManagement.Core.Customers.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.Contracts;
using System.Reflection.Emit;

namespace BankingApp.AccountManagement.Infrastructure.Configurations
{
    internal class AccountEntityConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {

            builder.ToTable("Accounts");
            
            builder.Property(e => e.AccountNo).IsRequired();
            builder.Property(e => e.CustomerId).IsRequired();
            builder.Property<int>("AccountTypeId").IsRequired();
            builder.Property<int>("AccountStatusId").IsRequired();

            builder.Property(p => p.DateAdded);
            builder.Property(p => p.CloasedDate);
            builder.Property(p => p.AccountUpdatedDate);
            builder.HasOne<Customer>()
              .WithMany()
              .IsRequired(true)
              .HasForeignKey("CustomerId");

        }
    }
}
