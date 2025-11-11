
using BankingApp.AccountManagement.Core.Accounts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.AccountManagement.Infrastructure.Configurations
{
    internal class CreditEntityConfiguration : IEntityTypeConfiguration<Credit>
    {
        public void Configure(EntityTypeBuilder<Credit> builder)
        {
            builder.ToTable("Credits");
            //Amount value object persisted as owned entity type supported since EF Core 2.0
            builder.OwnsOne(e => e.Amount, amountBuilder =>
            {
                amountBuilder.Property(e => e.Value)
                .HasColumnName("Value")
                .IsRequired();
            });
            builder.Property<Guid>("AccountId");
            builder.Property<DateTime>("CreatedAt");
            builder.Property<string>("Description").IsRequired().HasMaxLength(256);

        }
    }
}
