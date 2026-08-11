
using BankingAppDDD.Domains.Users.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingAppDDD.UserManagement.Infrastructure.Configurations
{
    internal class UserTypeEntityConfiguration : IEntityTypeConfiguration<UserType>
    {
        public void Configure(EntityTypeBuilder<UserType> builder)
        {
            builder.ToTable("UserTypes");
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
