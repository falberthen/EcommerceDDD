using System;
using EcommerceDDD.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceDDD.Infrastructure.Database.Configurations
{
    internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", "dbo");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
            .HasConversion(
                v => v.Value,
                v => new ProductId(v));

            builder.Property(e => e.CreationDate);
            builder.Property(e => e.Name)
            .HasMaxLength(25).IsRequired();

            builder.OwnsOne(e => e.Price, b =>
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
}
