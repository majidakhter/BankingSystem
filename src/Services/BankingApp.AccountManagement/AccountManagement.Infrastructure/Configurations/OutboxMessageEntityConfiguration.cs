using BankingAppDDD.Domains.Accounts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.AccountManagement.Infrastructure.Configurations
{
    internal class OutboxMessageEntityConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("outbox");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
            builder.Property(x => x.AggregateType).HasColumnName("aggregate_type").HasMaxLength(255).IsRequired();
            builder.Property(x => x.AggregateId).HasColumnName("aggregate_id").HasMaxLength(255).IsRequired();
            builder.Property(x => x.EventType).HasColumnName("event_type").HasMaxLength(255).IsRequired();
            builder.Property(x => x.Payload).HasColumnName("payload").HasColumnType("jsonb").IsRequired();
            builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
            builder.Property(x => x.ProcessedAt).HasColumnName("processed_at").IsRequired(false);

            builder.HasIndex(x => x.CreatedAt).HasDatabaseName("idx_outbox_created");
        }
    }
}
