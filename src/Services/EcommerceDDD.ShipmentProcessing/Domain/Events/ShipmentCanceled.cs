namespace EcommerceDDD.ShipmentProcessing.Domain.Events;

public record ShipmentCanceled : DomainEvent
{
    public Guid ShipmentId { get; private set; }
    public ShipmentCancellationReason ShipmentCancellationReason { get; private set; }

    public static ShipmentCanceled Create(
        Guid shipmentId,
        ShipmentCancellationReason shipmentCancellationReason)
    {
        if (shipmentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(shipmentId));

        return new ShipmentCanceled(shipmentId, shipmentCancellationReason);
    }

    private ShipmentCanceled(
        Guid shipmentId,
        ShipmentCancellationReason shipmentCancellationReason)
    {
        ShipmentId = shipmentId;
        ShipmentCancellationReason = shipmentCancellationReason;
    }
}