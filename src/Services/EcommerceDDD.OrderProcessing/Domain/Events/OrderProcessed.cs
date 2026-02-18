namespace EcommerceDDD.OrderProcessing.Domain.Events;

public record class OrderProcessed(
    Guid CustomerId,
    Guid OrderId,
    IReadOnlyList<OrderLineDetails> OrderLines,
    string CurrencyCode,
    decimal TotalPrice) : DomainEvent;

public record OrderLineDetails(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity);
