namespace EcommerceDDD.Orders.Application.Shipments.ProcessingShipment;

public class PackageShipped : IntegrationEvent
{
    public Guid ShipmentId { get; set; }
    public Guid OrderId { get; set; }
    public DateTime ShippedAt { get; set; }
}