namespace EcommerceDDD.OrderProcessing.Domain.Events;

[MessageIdentity(nameof(OrderPlaced))]
public record class OrderPlaced(
    Guid CustomerId,
    Guid OrderId,
    Guid QuoteId) : DomainEvent;
