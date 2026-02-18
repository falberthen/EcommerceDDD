namespace EcommerceDDD.PaymentProcessing.Domain.Events;

public record PaymentCanceled(
    Guid PaymentId,
    PaymentCancellationReason PaymentCancellationReason) : DomainEvent;
