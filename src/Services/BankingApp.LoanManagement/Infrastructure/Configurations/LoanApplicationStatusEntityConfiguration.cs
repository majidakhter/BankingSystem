
namespace BankingApp.LoanManagement.Infrastructure.Configurations
{
    /*internal class LoanApplicationStatusEntityConfiguration : IEntityTypeConfiguration<LoanApplicationStatus>
    {
        public void Configure(EntityTypeBuilder<LoanApplicationStatus> builder)
        {
            builder.ToTable("LoanApplicationStatus");

            //builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();

        }
    }*/
}
