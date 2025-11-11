
using BankingApp.AccountManagement.Core.Accounts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.AccountManagement.Infrastructure.Configurations
{
    internal class BeneficiaryEntityConfiguration : IEntityTypeConfiguration<BeneficiaryGroup>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryGroup> builder)
        {
            builder.ToTable("Beneficaries");
            builder.OwnsOne(e => e.Beneficiary, beneficiaryBuilder =>
            {
                beneficiaryBuilder.Property(e => e.BeneficaryName)
                .HasColumnName("BeneficaryName")
                .IsRequired();
                beneficiaryBuilder.Property(e => e.BeneficaryAccountNo)
               .HasColumnName("BeneficaryAccountNo")
               .IsRequired();
                beneficiaryBuilder.Property(e => e.BeneficaryBankName)
               .HasColumnName("BeneficaryBankName")
               .IsRequired();
            });
            
            builder.Property<Guid>("AccountId");
            builder.Property<DateTime>("AddedDate");
        }
    }
}
