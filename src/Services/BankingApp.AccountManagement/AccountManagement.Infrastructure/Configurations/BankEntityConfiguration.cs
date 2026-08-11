using BankingAppDDD.Domains.Banks.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.AccountManagement.Infrastructure.Configurations
{
    internal class BankEntityConfiguration : IEntityTypeConfiguration<Bank>
    {
        public void Configure(EntityTypeBuilder<Bank> builder)
        {
            builder.ToTable("Bank");
            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Property(p => p.DateAdded);
        }
    }
}
