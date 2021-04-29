using EcommerceDDD.Domain.Carts;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceDDD.Infrastructure.Database.Configurations
{
    internal sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Carts", "dbo");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(
                    v => v.Value,
                    v => new CartId(v));
            
            builder.Property(e => e.CustomerId)
                .HasConversion(guid => guid.Value, s => new CustomerId(s));

            builder.OwnsMany(s => s.Items, b =>
            {
                b.ToTable("CartItems", "dbo")
                    .HasKey(x => x.Id);

                b.Property(e => e.Id).IsRequired()
                    .ValueGeneratedNever();

                b.Property(e => e.ProductId)
                    .HasConversion(guid => guid.Value, s => new ProductId(s));

                b.Property(e => e.Quantity);
            });
        }
    }
}
