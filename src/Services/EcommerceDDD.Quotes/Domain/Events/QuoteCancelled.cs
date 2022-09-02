using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Quotes.Domain.Events;

public record class QuoteCancelled(
    QuoteId QuoteId,
    DateTime CancelledAt) : IDomainEvent
{
    public static QuoteCancelled Create(QuoteId quoteId, DateTime cancelledAt)
    {
        return new QuoteCancelled(
            quoteId,
            cancelledAt);
    }
}