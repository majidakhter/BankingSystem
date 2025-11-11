
using BankingApp.LoanManagement.Core.DebtInfos.Entities;
using BankingAppDDD.Domains.Abstractions.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.LoanManagement.Infrastructure.Configurations
{
    internal class DebtorInfoEntityConfiguration : IEntityTypeConfiguration<DebtorInfo>
    {
        public void Configure(EntityTypeBuilder<DebtorInfo> builder)
        {
            builder.ToTable("DebtorInfos");
            
            builder.Property(x => x.IdentificationNumber);
            builder.OwnsMany(x => x.Debts, debtBuilder =>
            {
                debtBuilder.Property(y=>y.Amount)
                      .HasConversion(x => x.Value, x =>  Amount.Create(x)); ;
            });
        }
    }
}
