using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Quotes.Domain.Events;

public record class QuoteConfirmed : IDomainEvent
{
    public Guid QuoteId { get; private set; }
    public DateTime ConfirmedAt { get; private set; }

    public static QuoteConfirmed Create(Guid quoteId, DateTime confirmedAt)
    {
        if (quoteId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(quoteId));
        if (confirmedAt == DateTime.MinValue)
            throw new ArgumentOutOfRangeException(nameof(confirmedAt));

        return new QuoteConfirmed(
            quoteId,
            confirmedAt);
    }
    private QuoteConfirmed(
        Guid quoteId,
        DateTime confirmedAt)
    {
        QuoteId = quoteId;
        ConfirmedAt = confirmedAt;
    }
}