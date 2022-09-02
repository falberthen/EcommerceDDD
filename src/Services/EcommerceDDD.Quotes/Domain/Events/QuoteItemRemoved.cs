using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Quotes.Domain.Events;

public record class QuoteItemRemoved(
    QuoteId QuoteId, 
    Guid QuoteItemId, 
    ProductId ProductId) : IDomainEvent
{
    public static QuoteItemRemoved Create(QuoteId quoteId, Guid quoteItemId, ProductId productId)
    {
        return new QuoteItemRemoved(
            quoteId,
            quoteItemId,
            productId);
    }
}