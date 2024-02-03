namespace EcommerceDDD.QuoteManagement.Domain.Events;

public record class QuoteConfirmed : DomainEvent
{
    public Guid QuoteId { get; private set; }

    public static QuoteConfirmed Create(Guid quoteId)
    {
        if (quoteId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(quoteId));

        return new QuoteConfirmed(quoteId);
    }

    private QuoteConfirmed(
        Guid quoteId)
    {
        QuoteId = quoteId;
    }
}