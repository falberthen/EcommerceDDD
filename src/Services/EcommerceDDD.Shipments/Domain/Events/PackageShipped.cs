using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Shipments.Domain.Events;

public record PackageShipped : IDomainEvent
{
    public Guid ShipmentId { get; private set; }
    public DateTime ShippedAt { get; private set; }

    public static PackageShipped Create(
        Guid shipmentId,
        DateTime shipedAt)
    {
        if (shipmentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(shipmentId));       
        if (shipedAt == DateTime.MinValue)
            throw new ArgumentOutOfRangeException(nameof(shipedAt));

        return new PackageShipped(
            shipmentId,
            shipedAt);
    }

    private PackageShipped(
        Guid shipmentId,
        DateTime shippedAt)
    {
        ShipmentId = shipmentId;
        ShippedAt = shippedAt;
    }
}