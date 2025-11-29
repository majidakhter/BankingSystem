using BankingApp.AccountManagement.Core.Accounts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.AccountManagement.Infrastructure.Configurations
{
    internal class AccountTypeEntityConfiguration : IEntityTypeConfiguration<AccountType>
    {
        public void Configure(EntityTypeBuilder<AccountType> builder)
        {
            builder.ToTable("AccountType");
            //builder.HasKey(o => o.Id);
            builder.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();
            builder.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();

        }
    }
}
