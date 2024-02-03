namespace EcommerceDDD.OrderProcessing.Application.Shipments.ProcessingShipment.IntegrationEvents;

public class ShipmentFinalized : IntegrationEvent
{
    public Guid ShipmentId { get; set; }
    public Guid OrderId { get; set; }
    public DateTime ShippedAt { get; set; }
}