namespace EcommerceDDD.ShipmentProcessing.Application.ProcessingShipment.IntegrationEvents;

[MessageIdentity(nameof(ShipmentNotDelivered))]
public class ShipmentNotDelivered : IntegrationEvent
{
    public Guid ShipmentId { get; private set; }
    public Guid OrderId { get; private set; }

    public ShipmentNotDelivered(
        Guid shipmentId,
        Guid orderId)
    {
        ShipmentId = shipmentId;
        OrderId = orderId;
    }
}
