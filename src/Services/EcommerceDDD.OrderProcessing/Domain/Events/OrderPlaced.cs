namespace EcommerceDDD.OrderProcessing.Domain.Events;

public record class OrderPlaced(
    Guid CustomerId,
    Guid OrderId,
    Guid QuoteId) : DomainEvent;
