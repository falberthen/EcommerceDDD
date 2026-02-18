namespace EcommerceDDD.QuoteManagement.Domain.Events;

public record class QuoteItemQuantityChanged(
    Guid QuoteId,
    Guid ProductId,
    int Quantity) : DomainEvent;
