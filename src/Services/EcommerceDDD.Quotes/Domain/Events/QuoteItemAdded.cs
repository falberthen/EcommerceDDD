using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Quotes.Domain.Events;

public record class QuoteItemAdded : IDomainEvent
{
    public Guid QuoteId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }

    public static QuoteItemAdded Create(QuoteItemData quoteItemData) 
    {
        if (quoteItemData.QuoteId.Value == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(quoteItemData.QuoteId));
        if (quoteItemData.ProductId.Value == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(quoteItemData.ProductId));
        if (quoteItemData.Quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quoteItemData.Quantity));

        return new QuoteItemAdded(
            quoteItemData.QuoteId.Value,
            quoteItemData.ProductId.Value,
            quoteItemData.Quantity);
    }

    private QuoteItemAdded(
        Guid quoteId,
        Guid productId,
        int quantity)
    {
        QuoteId = quoteId;
        ProductId = productId;
        Quantity = quantity;
    }
}
