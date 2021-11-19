using EcommerceDDD.Domain.Customers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceDDD.Infrastructure.Database.Configurations;

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
    }
}