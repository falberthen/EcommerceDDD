using EcommerceDDD.Products.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceDDD.Products.Infrastructure.Persistence;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

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
            b.Property(e => e.Amount)
            .HasColumnName("Price")
            .HasColumnType("decimal(5,2)");

            b.OwnsOne(e => e.Currency, bc =>
            {
                bc.Property(e => e.Code)
                .HasColumnName("CurrencyCode")
                .HasMaxLength(5).IsRequired();

                bc.Property(e => e.Symbol)
                .HasColumnName("CurrencySymbol")
                .HasMaxLength(5);
            });
        });
    }
}