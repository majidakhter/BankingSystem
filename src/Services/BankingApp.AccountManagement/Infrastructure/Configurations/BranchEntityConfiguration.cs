using BankingApp.AccountManagement.Core.Banks.Entities;
using BankingApp.AccountManagement.Core.Branches.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.AccountManagement.Infrastructure.Configurations
{
    internal class BranchEntityConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.ToTable("Branches");
            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Property(p => p.BranchCode).HasMaxLength(10).IsRequired();
            builder.Property(p => p.DateAdded);
            builder.Property(e => e.BankId).IsRequired();
            builder.HasOne<Bank>()
            .WithMany()
            .IsRequired(true)
            .HasForeignKey("BankId");
            builder.OwnsOne(e => e.BranchAddress, addressBuilder =>
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
            builder.OwnsOne(e => e.PhoneNumber, phoneBuilder =>
            {
                phoneBuilder.Property(e => e.Value)
                                  .HasColumnName("PhoneNumber")
                                  .IsRequired();
            });

        }
    }
}
