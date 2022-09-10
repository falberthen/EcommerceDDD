using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Quotes.Domain.Events;

public record class QuoteItemRemoved(
    QuoteId QuoteId, 
    ProductId ProductId) : IDomainEvent
{
    public static QuoteItemRemoved Create(QuoteId quoteId, ProductId productId)
    {
        return new QuoteItemRemoved(
            quoteId,
            productId);
    }
}