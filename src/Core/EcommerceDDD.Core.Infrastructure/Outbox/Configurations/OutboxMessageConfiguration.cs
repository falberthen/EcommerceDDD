using Microsoft.EntityFrameworkCore;
using EcommerceDDD.Core.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(x => x.Id);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.ProcessedAt);

        builder.Property(e => e.Type)
            .IsRequired();

        builder.Property(e => e.Data)
            .IsRequired();
    }
}