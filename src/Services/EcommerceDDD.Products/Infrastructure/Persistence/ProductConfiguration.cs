using EcommerceDDD.Products.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceDDD.Products.Infrastructure.Persistence;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", "dbo");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
        .HasConversion(
            v => v.Value,
            v => ProductId.Of(v));

        builder.Ignore(t => t.Version);

        builder.Property(e => e.Name)
        .HasMaxLength(25).IsRequired();

        builder.OwnsOne(e => e.UnitPrice, b =>
        {
            b.Property(e => e.Value)
            .HasColumnName("Price")
            .HasColumnType("decimal(5,2)");

            b.Property(e => e.CurrencyCode)
            .HasColumnName("Currency")
            .HasMaxLength(5)
            .IsRequired();
        });
    }
}