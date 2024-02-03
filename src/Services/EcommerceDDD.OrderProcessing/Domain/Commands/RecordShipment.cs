namespace EcommerceDDD.OrderProcessing.Domain.Commands;

public record class RecordShipment : ICommand
{
    public OrderId OrderId { get; private set; }
    public ShipmentId ShipmentId { get; private set; }

    public static RecordShipment Create(
        OrderId orderId,
        ShipmentId shipmentId)
    {
        if (orderId is null)
            throw new ArgumentNullException(nameof(orderId));
        if (shipmentId is null)
            throw new ArgumentNullException(nameof(shipmentId));

        return new RecordShipment(orderId, shipmentId);
    }

    private RecordShipment(
        OrderId orderId,
        ShipmentId shipmentId)
    {
        OrderId = orderId;
        ShipmentId = shipmentId;
    }
}