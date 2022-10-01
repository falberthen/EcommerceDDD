using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Shipments.Domain.Events;

namespace EcommerceDDD.Shipments.Infrastructure.Projections;

public class ShipmentDetails
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public IReadOnlyList<ProductItemDetails> ProductItems { get; set; } = default!;
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public ShipmentStatus Status { get; set; }

    public void Apply(PackageShipped shipped)
    {
        var productItems = shipped.ProductItems.Select(c =>
            new ProductItemDetails(
                c.ProductId,
                c.Quantity)
            ).ToList();

        Id = shipped.ShipmentId;
        OrderId = shipped.OrderId;
        ProductItems = productItems;
        ShippedAt = shipped.ShippedAt;
        Status = ShipmentStatus.Shipped;
    }

    public void Apply(PackageDelivered delivered)
    {
        DeliveredAt = delivered.DeliveredAt;
        Status = ShipmentStatus.Delivered;
    }
}