namespace EcommerceDDD.ShipmentProcessing.Domain.Commands;

public record class ProcessShipment : ICommand
{
    public ShipmentId ShipmentId { get; private set; }

    public static ProcessShipment Create(ShipmentId shipmentId)
    {
        if (shipmentId is null)
            throw new ArgumentNullException(nameof(shipmentId));
        
        return new ProcessShipment(shipmentId);
    }

    private ProcessShipment(ShipmentId shipmentId)
    {
        ShipmentId = shipmentId;
    }
}