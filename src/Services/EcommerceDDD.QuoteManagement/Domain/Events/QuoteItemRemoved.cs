namespace EcommerceDDD.QuoteManagement.Domain.Events;

public record class QuoteItemRemoved(
    Guid QuoteId,
    Guid ProductId) : DomainEvent;
