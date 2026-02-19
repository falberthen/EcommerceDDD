namespace EcommerceDDD.PaymentProcessing.Domain.Events;

public record PaymentCompleted(
    Guid PaymentId) : DomainEvent;
