using BankingAppDDD.CustomerManagement.Core.Customers.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace BankingAppDDD.CustomerManagement.Infrastructure.Configurations
{
    internal class CustomerEntityConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.Property<string>("Name").IsRequired().HasMaxLength(32);
            builder.Property<string>("Email").IsRequired().HasMaxLength(32);
            builder.Property<string>("PhoneNo").IsRequired().HasMaxLength(32);
            builder.Property(x=>x.CustomerTypeId).IsRequired();
            builder.Property(x => x.LoanStatus).HasConversion<string>();
            builder.Property<DateTime>("DateAdded");
            builder.Property<DateTime>("UpdatedOn");
            builder.OwnsOne(e => e.DateOfBirth, birthdateBuilder => {
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
