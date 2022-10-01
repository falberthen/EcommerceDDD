using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Orders.Application.Shipments.FinalizingShipment;

public class OrderDelivered : IIntegrationEvent
{
    public Guid ShipmentId { get; set; }
    public Guid OrderId { get; set; }
    public DateTime DeliveredAt { get; set; }
}