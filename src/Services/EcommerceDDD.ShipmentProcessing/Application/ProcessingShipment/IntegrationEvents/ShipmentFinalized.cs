namespace EcommerceDDD.ShipmentProcessing.Application.ProcessingShipment.IntegrationEvents;

public class ShipmentFinalized : IntegrationEvent
{
    public Guid ShipmentId { get; private set; }
    public Guid OrderId { get; private set; }
    public DateTime ShippedAt { get; private set; }

    public ShipmentFinalized(
        Guid shipmentId,
        Guid orderId,
        DateTime shippedAt)
    {
        ShipmentId = shipmentId;
        OrderId = orderId;
        ShippedAt = shippedAt;
    }
}