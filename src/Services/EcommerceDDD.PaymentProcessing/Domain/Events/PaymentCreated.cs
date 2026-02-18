namespace EcommerceDDD.PaymentProcessing.Domain.Events;

public record PaymentCreated(
    Guid PaymentId,
    Guid CustomerId,
    Guid OrderId,
    decimal TotalAmount,
    string CurrencyCode,
    IReadOnlyList<ProductItemDetails> ProductItems) : DomainEvent;

public record ProductItemDetails(Guid ProductId, int Quantity);
