using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Shipments.Domain.Events;

public record PackageDelivered : IDomainEvent
{
    public Guid ShipmentId { get; private set; }
    public Guid OrderId { get; private set; }
    public DateTime DeliveredAt { get; private set; }

    public static PackageDelivered Create(
        Guid shipmentId,
        Guid orderId,
        DateTime deliveredAt)
    {        
        if (shipmentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(shipmentId));
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (deliveredAt == DateTime.MinValue)
            throw new ArgumentOutOfRangeException(nameof(deliveredAt));

        return new PackageDelivered(shipmentId, orderId, deliveredAt);
    }

    private PackageDelivered(
        Guid shipmentId,
        Guid orderId,
        DateTime deliveredAt)
    {
        ShipmentId = shipmentId;
        OrderId = orderId;
        DeliveredAt = deliveredAt;
    }
}
