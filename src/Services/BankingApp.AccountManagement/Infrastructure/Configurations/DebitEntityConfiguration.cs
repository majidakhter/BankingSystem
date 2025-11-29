using BankingApp.AccountManagement.Core.Accounts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.AccountManagement.Infrastructure.Configurations
{
    internal class DebitEntityConfiguration : IEntityTypeConfiguration<Debit>
    {
        public void Configure(EntityTypeBuilder<Debit> builder)
        {
            builder.ToTable("Debits");
            //Amount value object persisted as owned entity type supported since EF Core 2.0
            builder.OwnsOne(e => e.Amount, amountBuilder =>
            {
                amountBuilder.Property(e => e.Value)
                .HasColumnName("CreditAmount")
                .IsRequired();
            });
            builder.Property<Guid>("AccountId");
            builder.Property<DateTime>("CreatedAt");
            builder.Property<string>("Description").IsRequired().HasMaxLength(32);
        }
    }
}
