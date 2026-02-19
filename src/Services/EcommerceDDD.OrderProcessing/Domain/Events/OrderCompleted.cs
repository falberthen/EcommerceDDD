namespace EcommerceDDD.OrderProcessing.Domain.Events;

public record OrderCompleted(
    Guid OrderId,
    Guid ShipmentId) : DomainEvent;
