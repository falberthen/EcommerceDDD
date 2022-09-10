using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Quotes.Domain.Events;

public record class QuoteItemAdded(
    QuoteId QuoteId,
    ProductId ProductId,
    int Quantity) : IDomainEvent
{
    public static QuoteItemAdded Create(QuoteItemData quoteItemData) 
    {
        return new QuoteItemAdded(
            quoteItemData.QuoteId, 
            quoteItemData.ProductId,
            quoteItemData.Quantity);
    }
}
