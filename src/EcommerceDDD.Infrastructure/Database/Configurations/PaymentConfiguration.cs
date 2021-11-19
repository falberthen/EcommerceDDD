using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Orders;
using EcommerceDDD.Domain.Payments;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EcommerceDDD.Infrastructure.Database.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments", "dbo");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
        .HasConversion(
            v => v.Value,
            v => new PaymentId(v));

        builder.Property(e => e.CustomerId)
        .HasConversion(guid => guid.Value, s => new CustomerId(s));

        builder.Property(e => e.OrderId)
        .HasConversion(guid => guid.Value, s => new OrderId(s));

        builder.Property<DateTime>("CreatedAt")
        .HasColumnName("CreatedAt");

        builder.Property<DateTime?>("PaidAt")
        .HasColumnName("PaidAt");

        builder.Property("Status")
        .HasColumnName("StatusId")
        .HasConversion(new EnumToNumberConverter<PaymentStatus, byte>());
    }
}