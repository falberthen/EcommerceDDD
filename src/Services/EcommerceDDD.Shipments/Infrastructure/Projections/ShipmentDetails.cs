using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Shipments.Domain.Events;

namespace EcommerceDDD.Shipments.Infrastructure.Projections;

public class ShipmentDetails
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public IReadOnlyList<ProductItemDetails> ProductItems { get; set; } = default!;
    public DateTime? CreatedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public ShipmentStatus Status { get; set; }

    public void Apply(ShipmentCreated shipped)
    {
        var productItems = shipped.ProductItems.Select(c =>
            new ProductItemDetails(
                c.ProductId,
                c.Quantity)
            ).ToList();

        Id = shipped.ShipmentId;
        OrderId = shipped.OrderId;
        ProductItems = productItems;
        CreatedAt = shipped.CreatedAt;
        Status = ShipmentStatus.Shipped;
    }

    public void Apply(PackageShipped shipped)
    {
        ShippedAt = shipped.ShippedAt;
        Status = ShipmentStatus.Shipped;
    }
}