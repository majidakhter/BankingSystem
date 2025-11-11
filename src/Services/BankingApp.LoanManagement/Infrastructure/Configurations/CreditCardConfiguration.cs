using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.LoanManagement.Infrastructure.Configurations
{
    internal class CreditCardConfiguration
    {
       /* public void Configure(EntityTypeBuilder<CreditCards> builder)
        {
            builder.Property(e => e.CardNumber)
                   .HasMaxLength(50)
                   .IsRequired();
            builder.Property(e => e.CardType)
                   .HasMaxLength(50)
                   .IsRequired();
            builder.Property(e => e.ExpiryDate)
                   .HasMaxLength(50)
                   .IsRequired();
            builder.Property(e => e.CVV)
                   .HasMaxLength(50)
                   .IsRequired();
            builder.Property(e => e.CreditLimit)
                   .HasMaxLength(50)
                   .IsRequired();
            builder.HasOne<Customer>()
                   .WithMany()
                   .HasForeignKey(e => e.CustomerId)
                   .IsRequired();
        }*/
    }
}
