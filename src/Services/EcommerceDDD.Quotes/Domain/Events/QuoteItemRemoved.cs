using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Quotes.Domain.Events;

public record class QuoteItemRemoved : IDomainEvent
{
    public Guid QuoteId { get; private set; }
    public Guid ProductId { get; private set; }

    public static QuoteItemRemoved Create(Guid quoteId, Guid productId)
    {
        if (quoteId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(quoteId));
        if (productId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(productId));
        
        return new QuoteItemRemoved(
            quoteId,
            productId);
    }

    private QuoteItemRemoved(
        Guid quoteId,
        Guid productId)
    {
        QuoteId = quoteId;
        ProductId = productId;
    }
}