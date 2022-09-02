using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Quotes.Domain.Events;

public record class QuoteItemQuantityChanged(
    Guid Id,
    QuoteId QuoteId,
    ProductId ProductId,
    int Quantity) : IDomainEvent
{
    public static QuoteItemQuantityChanged Create(QuoteItemData quoteItemData) 
    {
        return new QuoteItemQuantityChanged(
            quoteItemData.Id,
            quoteItemData.QuoteId, 
            quoteItemData.ProductId,
            quoteItemData.Quantity);
    }
}
