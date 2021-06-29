using EcommerceDDD.Domain.Core.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceDDD.Infrastructure.Database.Configurations
{
    internal sealed class StoredMessageConfiguration : IEntityTypeConfiguration<StoredEvent>
    {
        public void Configure(EntityTypeBuilder<StoredEvent> builder)
        {
            builder.ToTable("StoredEvents", "dbo");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.Property(r => r.ProcessedAt);

            builder.Property(r => r.MessageType)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(r => r.Payload)
                .IsRequired();
        }
    }
}
