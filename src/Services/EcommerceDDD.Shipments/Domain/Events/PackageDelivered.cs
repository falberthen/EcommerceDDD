using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Shipments.Domain.Events;

public record PackageDelivered(
    ShipmentId ShipmentId,
    OrderId OrderId,
    DateTime DeliveredAt) : IDomainEvent
{
    public static PackageDelivered Create(
        ShipmentId shipmentId,
        OrderId orderId,
        DateTime deliveredAt)
    {
        return new PackageDelivered(shipmentId, orderId, deliveredAt);
    }
}
