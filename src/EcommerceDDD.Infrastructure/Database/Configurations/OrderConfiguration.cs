using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Orders;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.Quotes;
using EcommerceDDD.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EcommerceDDD.Infrastructure.Database.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders", "dbo");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
        .HasConversion(
            v => v.Value,
            v => new OrderId(v));

        builder.Property(x => x.CustomerId)
        .HasConversion(
            v => v.Value,
            v => new CustomerId(v));

        builder.Property(x => x.QuoteId)
        .HasConversion(
            v => v.Value,
            v => new QuoteId(v));

        builder.Property<DateTime>("CreatedAt")
        .HasColumnName("CreatedAt");

        builder.Property("Status")
        .HasColumnName("StatusId")
        .HasConversion(new EnumToNumberConverter<OrderStatus, byte>());

        builder.OwnsOne(o => o.TotalPrice, o =>
        {
            o.Property(e => e.Value)
            .HasColumnName("TotalPrice")
            .HasColumnType("decimal(5,2)");

            o.Property(e => e.CurrencyCode)
            .HasColumnName("Currency").HasMaxLength(5);
        });

        builder.OwnsMany(s => s.OrderLines, y =>
        {
            y.WithOwner().HasForeignKey("OrderId");

            y.ToTable("OrderLines", "dbo");

            y.Property(x => x.OrderId)
            .HasConversion(
                v => v.Value,
                v => new OrderId(v));

            y.Property(x => x.ProductId)
            .HasConversion(
                v => v.Value,
                v => new ProductId(v));

            y.HasKey("OrderId", "ProductId");

            y.OwnsOne<Money>("ProductBasePrice", b =>
            {
                b.Property(e => e.Value)
                .HasColumnName("BasePrice")
                .HasColumnType("decimal(5,2)");

                b.Property(e => e.CurrencyCode)
                .HasColumnName("BaseCurrency")
                .HasMaxLength(5);
            });

            y.OwnsOne<Money>("ProductExchangePrice", b =>
            {
                b.Property(e => e.Value)
                .HasColumnName("ExchangePrice")
                .HasColumnType("decimal(5,2)");

                b.Property(e => e.CurrencyCode)
                .HasColumnName("ExchangeCurrency")
                .HasMaxLength(5);
            });
        });
    }
}