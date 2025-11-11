using BankingApp.AccountManagement.Core.Banks.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace BankingApp.AccountManagement.Infrastructure.Configurations
{
    internal class BankEntityConfiguration : IEntityTypeConfiguration<Bank>
    {
        public void Configure(EntityTypeBuilder<Bank> builder)
        {
            builder.ToTable("Bank");
            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Property(p => p.DateAdded);
            builder.OwnsOne(e => e.PhoneNumber, phoneBuilder =>
            {
                phoneBuilder.Property(e => e.Value)
                                  .HasColumnName("PhoneNumber")
                                  .IsRequired();
            });

            builder.OwnsOne(e => e.BankAddress, addressBuilder =>
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
