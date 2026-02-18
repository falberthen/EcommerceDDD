namespace EcommerceDDD.OrderProcessing.Domain.Events;

public record class OrderCanceled(
    Guid OrderId,
    Guid? PaymentId,
    OrderCancellationReason OrderCancellationReason,
    string OrderCancellationReasonDescription) : DomainEvent;
