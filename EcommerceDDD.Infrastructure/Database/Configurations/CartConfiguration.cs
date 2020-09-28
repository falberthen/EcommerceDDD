using System;
using EcommerceDDD.Domain.Carts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceDDD.Infrastructure.Database.Configurations
{
    internal sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Carts", "dbo");

            builder.HasKey(r => r.Id);

            builder.OwnsMany(s => s.Items, b =>
            {
                b.Property(p => p.Id).IsRequired().ValueGeneratedNever();

                b.ToTable("CartItems", "dbo")
                    .HasKey("Id");

                b.Property(e => e.Quantity);

                b.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey("ProductId");
            });

            var navigation = builder.Metadata.FindNavigation(nameof(Cart.Items));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
