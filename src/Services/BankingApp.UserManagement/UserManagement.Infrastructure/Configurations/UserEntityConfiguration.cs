using BankingAppDDD.UserManagement.Core.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingAppDDD.UserManagement.Infrastructure.Configurations
{
    internal class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.Property(x => x.KeyCloakUserId).IsRequired();
            builder.Property<string>("FirstName").IsRequired().HasMaxLength(32);
            builder.Property<string>("LastName").IsRequired().HasMaxLength(32);
            builder.Property<string>("Email").IsRequired().HasMaxLength(32);
            builder.Property<string>("PhoneNo").IsRequired().HasMaxLength(32);
            builder.Property(x => x.UserTypeId).IsRequired();
            builder.Property(x => x.Gender).IsRequired();
            builder.Property(x => x.SSN).IsRequired();
            builder.Property(x => x.ProfileImage);
            builder.Property(x => x.LoanStatus).HasConversion<string>();
            builder.Ignore(x => x.BranchId);
            builder.Property<DateTime>("DateAdded");

            builder.Property<DateTime>("UpdatedOn");
            builder.OwnsOne(e => e.DateOfBirth, birthdateBuilder =>
            {
                birthdateBuilder.Property(e => e.Value)
                                  .HasColumnName("DateOfBirth")
                                  .IsRequired();
            });
            builder.OwnsOne(e => e.PermanentAddress, addressBuilder =>
            {
                addressBuilder.Property(e => e.Street)
                                  .HasColumnName("Street")
                                  .IsRequired();
                addressBuilder.Property(e => e.City)
                                  .HasColumnName("City")
                                  .IsRequired();
                addressBuilder.Property(e => e.State)
                                  .HasColumnName("State")
                                  .IsRequired();
                addressBuilder.Property(e => e.ZipCode)
                                  .HasColumnName("ZipCode")
                                  .IsRequired();
                addressBuilder.Property(e => e.Country)
                                  .HasColumnName("Country")
                                  .IsRequired();
            });

        }
    }
}
