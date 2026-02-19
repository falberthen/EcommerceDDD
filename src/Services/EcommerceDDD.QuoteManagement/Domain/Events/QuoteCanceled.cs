namespace EcommerceDDD.QuoteManagement.Domain.Events;

public record class QuoteCanceled(
    Guid QuoteId) : DomainEvent;
