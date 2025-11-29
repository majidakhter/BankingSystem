using BankingApp.LoanManagement.Core.LoanApplicationsEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.LoanManagement.Infrastructure.Configurations
{
    internal class OperatorConfiguration : IEntityTypeConfiguration<Operator>
    {
        public void Configure(EntityTypeBuilder<Operator> builder)
        {
            builder.ToTable("Operators");

            //builder.Property(x => x.UnderWriterId);

            builder.OwnsOne(x => x.CompetenceLevel, competencyLevelBuilder =>
            {
                competencyLevelBuilder.Property(x => x.Value).HasColumnName("CompetenceLevelAmount");
            });

        }
    }
}
