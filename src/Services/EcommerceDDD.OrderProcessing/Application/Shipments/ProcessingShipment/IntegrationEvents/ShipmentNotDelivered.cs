namespace EcommerceDDD.OrderProcessing.Application.Shipments.ProcessingShipment.IntegrationEvents;

[MessageIdentity(nameof(ShipmentNotDelivered))]
public class ShipmentNotDelivered : IntegrationEvent
{
    public Guid ShipmentId { get; set; }
    public Guid OrderId { get; set; }
}
