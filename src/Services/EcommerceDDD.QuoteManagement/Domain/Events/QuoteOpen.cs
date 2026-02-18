namespace EcommerceDDD.QuoteManagement.Domain.Events;

public record class QuoteOpen(
    Guid QuoteId,
    Guid CustomerId,
    string CurrencyCode) : DomainEvent;
