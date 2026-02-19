namespace EcommerceDDD.OrderProcessing.Domain.Events;

public record OrderShipped(
    Guid OrderId,
    Guid ShipmentId,
    IList<Guid> OrderLineProducts) : DomainEvent;
