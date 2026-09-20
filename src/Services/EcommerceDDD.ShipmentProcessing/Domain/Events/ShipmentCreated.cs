namespace EcommerceDDD.ShipmentProcessing.Domain.Events;

public record ShipmentCreated(
    Guid ShipmentId,
    Guid OrderId,
    string ShippingAddress,
    IReadOnlyList<ProductItemDetails> ProductItems) : DomainEvent;

public record ProductItemDetails(Guid ProductId, int Quantity);
