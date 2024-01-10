namespace EcommerceDDD.Shipments.Domain.Events;

public record ShipmentCanceled : IDomainEvent
{
    public Guid ShipmentId { get; private set; }
    public DateTime CanceledAt { get; private set; }
    public ShipmentCancellationReason ShipmentCancellationReason { get; private set; }

    public static ShipmentCanceled Create(
        Guid shipmentId,
        DateTime canceledAt,
        ShipmentCancellationReason shipmentCancellationReason)
    {
        if (shipmentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(shipmentId));
        if (canceledAt == DateTime.MinValue)
            throw new ArgumentOutOfRangeException(nameof(canceledAt));

        return new ShipmentCanceled(shipmentId, canceledAt, shipmentCancellationReason);
    }

    private ShipmentCanceled(
        Guid shipmentId,
        DateTime canceledAt,
        ShipmentCancellationReason shipmentCancellationReason)
    {
        ShipmentId = shipmentId;
        CanceledAt = canceledAt;
        ShipmentCancellationReason = shipmentCancellationReason;
    }
}