using BankingAppDDD.Domains.Accounts.Entities;
using BankingAppDDD.Domains.CustomerAccounts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.AccountManagement.Infrastructure.Configurations
{
    internal class AccountEntityConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");
            builder.Property(e => e.AccountNo).IsRequired();

            //builder.Property(e => e.CurrentBalance).IsRequired();
            //builder.Property(e => e.UserId).IsRequired();
            builder.HasOne<UserAccount>()
              .WithMany()
              .IsRequired(true)
              .HasForeignKey("UserId");
            builder.Property(e => e.KeycloakUserId).IsRequired();
            builder.Property<int>("AccountTypeId").IsRequired();
            builder.Property<int>("AccountStatusId").IsRequired();
            builder.Property(p => p.DateAdded);
            builder.Property(p => p.ClosedDate);
            builder.Property(p => p.AccountUpdatedDate);
            

        }
    }
}
