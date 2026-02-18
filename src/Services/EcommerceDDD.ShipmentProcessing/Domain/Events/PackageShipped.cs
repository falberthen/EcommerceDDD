namespace EcommerceDDD.ShipmentProcessing.Domain.Events;

public record PackageShipped(
    Guid ShipmentId) : DomainEvent;
