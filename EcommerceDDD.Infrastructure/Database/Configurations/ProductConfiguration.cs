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

            builder.HasKey(r => r.Id);
            builder.Property(e => e.CreationDate);
            builder.Property(e => e.Name).HasMaxLength(25).IsRequired();

            builder.OwnsOne(e => e.Price, b =>
            {
                b.Property(e => e.Value).HasColumnName("Price").HasColumnType("decimal(5,2)");
                b.OwnsOne(e => e.Currency, bc =>
                {
                    bc.Property(e => e.Name).HasColumnName("Currency").HasMaxLength(5).IsRequired();
                    bc.Property(e => e.Symbol).HasColumnName("CurrencySymbol").HasMaxLength(5).IsRequired();
                });
            });
        }
    }
}  
