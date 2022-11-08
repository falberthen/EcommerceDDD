using Microsoft.EntityFrameworkCore;
using EcommerceDDD.Core.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Payload)            
            .HasColumnType("jsonb");

        builder.Property(e => e.AggregateId);

        builder.Property(e => e.AggregateType);

        builder.Property(e => e.Type);

        builder.Property(e => e.ProcessedAt);

        builder.Property(e => e.Timestamp)
            .HasColumnType("Timestamp");                    
    }
}
