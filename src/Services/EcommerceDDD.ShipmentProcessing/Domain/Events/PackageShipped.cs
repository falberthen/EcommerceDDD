namespace EcommerceDDD.ShipmentProcessing.Domain.Events;

public record PackageShipped : DomainEvent
{
    public Guid ShipmentId { get; private set; }

    public static PackageShipped Create(
        Guid shipmentId)
    {
        if (shipmentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(shipmentId));       

        return new PackageShipped(shipmentId);
    }

    private PackageShipped(Guid shipmentId)
    {
        ShipmentId = shipmentId;
    }
}