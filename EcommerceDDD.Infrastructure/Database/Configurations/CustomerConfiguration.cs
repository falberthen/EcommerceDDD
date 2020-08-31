using System;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Customers.Orders;
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

            builder.HasKey(b => b.Id);

            builder.Property(c => c.Email)
                .HasColumnType("varchar(100)")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Name)
                .HasColumnType("varchar(100)")
                .HasMaxLength(100)
                .IsRequired();

            builder.OwnsMany<Order>("Orders", x =>
            {
                x.WithOwner().HasForeignKey("CustomerId");

                x.ToTable("Orders", "dbo");

                x.Property<Guid>("Id");
                x.HasKey("Id");
                x.Property<DateTime>("CreatedAt").HasColumnName("CreatedAt");
                x.Property<DateTime?>("ChangedAt").HasColumnName("ChangeDate");
                x.Property<bool>("IsCancelled").HasColumnName("IsCancelled");

                x.Property("Status").HasColumnName("StatusId").HasConversion(new EnumToNumberConverter<OrderStatus, byte>());

                x.OwnsOne(e => e.TotalPrice, b =>
                {
                    b.Property(e => e.Value).HasColumnName("TotalPrice").HasColumnType("decimal(5,2)");
                    b.OwnsOne(e => e.Currency, bc =>
                    {
                        bc.Property(e => e.Name).HasColumnName("Currency").HasMaxLength(5).IsRequired();
                    });
                });

                x.OwnsMany<OrderLine>("OrderLines", y =>
                {
                    y.WithOwner().HasForeignKey("OrderId");

                    y.ToTable("OrderLines", "dbo");
                    y.Property<Guid>("OrderId");
                    y.Property<Guid>("ProductId");

                    y.HasKey("OrderId", "ProductId");

                    y.OwnsOne(e => e.ProductBasePrice, b =>
                    {
                        b.Property(e => e.Value).HasColumnName("BasePrice").HasColumnType("decimal(5,2)");
                        b.OwnsOne(e => e.Currency, bc =>
                        {
                            bc.Property(e => e.Name).HasColumnName("BaseCurrency").HasMaxLength(5);
                        });
                    });

                    y.OwnsOne(e => e.ProductExchangePrice, b =>
                    {
                        b.Property(e => e.Value).HasColumnName("ExchangePrice").HasColumnType("decimal(5,2)");
                        b.OwnsOne(e => e.Currency, bc =>
                        {
                            bc.Property(e => e.Name).HasColumnName("ExchangeCurrency").HasMaxLength(5);
                        });
                    });
                });
            });
        }
    }
}
