using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Quotes.Domain.Events;

public record class QuoteConfirmed(
    QuoteId QuoteId,
    DateTime ConfirmedAt) : IDomainEvent
{
    public static QuoteConfirmed Create(QuoteId quoteId, DateTime confirmedAt)
    {
        return new QuoteConfirmed(
            quoteId,
            confirmedAt);
    }
}