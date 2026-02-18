namespace EcommerceDDD.QuoteManagement.Domain.Events;

public record class QuoteConfirmed(
    Guid QuoteId) : DomainEvent;
