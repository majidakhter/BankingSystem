//using global::Credit.Core.LoanApplications.Entities;
//using global::Credit.Core.LoanApplications.ValueObjects;
using BankingApp.LoanManagement.Core.LoanApplications.ValueObjects;
using BankingApp.LoanManagement.Core.LoanApplicationsEntities;
using BankingAppDDD.Domains.Abstractions.ValueObjects;
using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    namespace BankingApp.LoanManagement.Infrastructure.Configurations
    {
        internal class LoanApplicationEntityConfiguration : IEntityTypeConfiguration<LoanApplication>
        {
            public void Configure(EntityTypeBuilder<LoanApplication> builder)
            {

                builder.ToTable("LoanApplications");
                builder
                    .Property(x => x.Number)
                    .HasConversion(x => x.Number, x => new LoanApplicationNumber(x));

                builder
                .Property(x => x.Status)
                .HasConversion<string>();
                builder.Property<int>("LoanTypeId").IsRequired();
                builder.OwnsOne(x => x.Score, opts =>
                {
                    opts.Property(x => x.Explanation);
                    opts.Property(x => x.Score).HasColumnName("ScoringResult").HasConversion<string>();
                });
                builder.OwnsOne(e => e.Customer, customerBuilder =>
                {
                    customerBuilder.Property(e => e.CustomerId)
                                      .HasColumnName("CustomerId")
                                      .IsRequired();
                    customerBuilder.Property(e => e.BirthDate)
                                      .HasColumnName("BirthDate")
                                      .IsRequired();
                    customerBuilder.OwnsOne(x => x.MonthlyIncome, amountBuilder =>
                    {
                        amountBuilder.Property(e => e.Value)
                                      .HasColumnName("MonthlyIncomeAmount");
                    });
                }).Navigation(x => x.Customer).IsRequired();

                builder.OwnsOne(x => x.Asset, opts =>
                {
                    opts.Property(x => x.Value)
                    .HasConversion(x => x.Value, x => Amount.Create(x))
                        .HasColumnName("PropertyValueAmount");

                    opts.OwnsOne(x => x.Address, addr =>
                    {
                        addr.Property(x => x.Country).IsRequired();
                        addr.Property(x => x.ZipCode).IsRequired();
                        addr.Property(x => x.City).IsRequired();
                        addr.Property(x => x.Street).IsRequired();
                    });
                });

                builder.OwnsOne(x => x.Loan, opts =>
                {
                    opts.Property(x => x.LoanNumberOfYears).IsRequired();
                    opts.OwnsOne(z => z.LoanAmount, amountBuilder =>
                    {
                        amountBuilder.Property(m => m.Value).HasColumnName("LoanAmount").IsRequired();
                    });
                    opts.OwnsOne(z => z.InterestRate, interestBuilder =>
                    {
                        interestBuilder.Property(m => m.Value).HasColumnName("InterestRate").IsRequired();
                       
                    });
                });

                builder.OwnsOne(x => x.Decision, opts =>
                {
                    opts.Property(x => x.DecisionDate);
                    opts.Property(x => x.DecisionBy)
                        .HasColumnName("DecisionBy");
                });

                builder.OwnsOne(x => x.Registration, opts =>
                {
                    opts.Property(x => x.RegistrationDate);
                    opts.Property(z => z.RegisteredBy);
                    
                });

            }
        }
    }


