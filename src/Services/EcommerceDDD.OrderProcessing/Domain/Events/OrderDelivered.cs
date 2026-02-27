namespace EcommerceDDD.OrderProcessing.Domain.Events;

public record OrderDelivered(
    Guid OrderId,
    Guid ShipmentId) : DomainEvent;
