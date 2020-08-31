using System;
using System.Collections.Generic;
using System.Text;
using EcommerceDDD.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EcommerceDDD.Infrastructure.Database.Configurations
{
    internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments", "dbo");

            builder.HasKey(b => b.Id);
            
            builder.Property<Guid>("OrderId").HasColumnName("OrderId");
            builder.Property<DateTime>("CreatedAt").HasColumnName("CreatedAt");
            builder.Property<DateTime?>("PaidAt").HasColumnName("PaidAt");
            builder.Property("Status").HasColumnName("StatusId").HasConversion(new EnumToNumberConverter<PaymentStatus, byte>());
            builder.Property<bool>("ConfirmationEmailSent").HasColumnName("ConfirmationEmailSent");
        }
    }
}
