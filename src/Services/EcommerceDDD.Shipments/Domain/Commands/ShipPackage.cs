namespace EcommerceDDD.Shipments.Domain.Commands;

public record class ShipPackage : ICommand
{
    public ShipmentId ShipmentId { get; private set; }

    public static ShipPackage Create(ShipmentId shipmentId)
    {
        if (shipmentId is null)
            throw new ArgumentNullException(nameof(shipmentId));
        
        return new ShipPackage(shipmentId);
    }

    private ShipPackage(ShipmentId shipmentId)
    {
        ShipmentId = shipmentId;
    }
}