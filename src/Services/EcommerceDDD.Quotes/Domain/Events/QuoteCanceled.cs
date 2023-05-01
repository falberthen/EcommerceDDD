namespace EcommerceDDD.Quotes.Domain.Events;

public record class QuoteCanceled : IDomainEvent
{
    public Guid QuoteId { get; private set; }
    public DateTime CancelledAt { get; private set; }

    public static QuoteCanceled Create(Guid quoteId, DateTime cancelledAt)
    {
        if (quoteId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(quoteId));
        if (cancelledAt == DateTime.MinValue)
            throw new ArgumentOutOfRangeException(nameof(cancelledAt));
       
        return new QuoteCanceled(
            quoteId,
            cancelledAt);
    }

    private QuoteCanceled(
        Guid quoteId,
        DateTime cancelledAt)
    {
        QuoteId = quoteId;
        CancelledAt = cancelledAt;
    }
}