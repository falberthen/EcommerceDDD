namespace EcommerceDDD.QuoteManagement.Domain.Events;

public record class QuoteCanceled : DomainEvent
{
    public Guid QuoteId { get; private set; }

    public static QuoteCanceled Create(Guid quoteId)
    {
        if (quoteId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(quoteId));
       
        return new QuoteCanceled(quoteId);
    }

    private QuoteCanceled(Guid quoteId)
    {
        QuoteId = quoteId;
    }
}