namespace EcommerceDDD.ShipmentProcessing.Domain.Events;

public record ShipmentCanceled(
    Guid ShipmentId,
    ShipmentCancellationReason ShipmentCancellationReason) : DomainEvent;
