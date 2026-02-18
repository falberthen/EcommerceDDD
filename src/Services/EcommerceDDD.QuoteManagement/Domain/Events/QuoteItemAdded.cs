namespace EcommerceDDD.QuoteManagement.Domain.Events;

public record class QuoteItemAdded(
    Guid QuoteId,
    Guid ProductId,
    string ProductName,
    decimal ProductPrice,
    int Quantity) : DomainEvent;
