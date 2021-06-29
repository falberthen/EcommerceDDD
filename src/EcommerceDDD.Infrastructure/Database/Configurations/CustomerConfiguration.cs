using System;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Customers.Orders;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EcommerceDDD.Infrastructure.Database.Configurations
{
    internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers", "dbo");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(
                    v => v.Value,
                    v => new CustomerId(v));

            builder.Property(c => c.Email)
                .HasColumnType("varchar(100)")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Name)
                .HasColumnType("varchar(100)")
                .HasMaxLength(100)
                .IsRequired();

            builder.OwnsMany(c => c.Orders, o =>
            {
                o.ToTable("Orders", "dbo");

                o.WithOwner()
                    .HasForeignKey("CustomerId");

                o.HasKey(x => x.Id);
                o.Property(x => x.Id)
                    .HasConversion(
                        v => v.Value,
                        v => new OrderId(v));

                o.Property<DateTime>("CreatedAt")
                    .HasColumnName("CreatedAt");

                o.Property("Status")
                    .HasColumnName("StatusId")
                    .HasConversion(new EnumToNumberConverter<OrderStatus, byte>());

                o.OwnsOne(o => o.TotalPrice, o =>
                {
                    o.Property(e => e.Value)
                        .HasColumnName("TotalPrice")
                        .HasColumnType("decimal(5,2)");

                    o.Property(e => e.CurrencyCode)
                        .HasColumnName("Currency").HasMaxLength(5);
                });

                o.OwnsMany(s => s.OrderLines, y =>
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
            });
        }
    }
}
