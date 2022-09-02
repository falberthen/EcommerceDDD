using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Shipments.Domain.Events;

public record PackageShipped(
    ShipmentId ShipmentId,
    OrderId OrderId,
    IReadOnlyList<ProductItem> ProductItems,
    DateTime ShippedAt
    ) : IDomainEvent
{
    public static PackageShipped Create(
        ShipmentId shipmentId,
        OrderId orderId, 
        IReadOnlyList<ProductItem> productItems,
        DateTime shippedAt)
    {
        return new PackageShipped(shipmentId, orderId, productItems, shippedAt);
    }
}
