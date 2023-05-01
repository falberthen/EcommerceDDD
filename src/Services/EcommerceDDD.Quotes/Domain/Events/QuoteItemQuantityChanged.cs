namespace EcommerceDDD.Quotes.Domain.Events;

public record class QuoteItemQuantityChanged : IDomainEvent
{
    public Guid QuoteId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }

    public static QuoteItemQuantityChanged Create(QuoteItemData quoteItemData)
    {
        var (QuoteId, ProductId, Quantity) = quoteItemData;

        if (QuoteId is null)
            throw new ArgumentNullException(nameof(QuoteId));
        if (ProductId is null)
            throw new ArgumentNullException(nameof(ProductId));        
        if (Quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(Quantity));

        return new QuoteItemQuantityChanged(
            QuoteId.Value,
            ProductId.Value,
            Quantity);
    }
    private QuoteItemQuantityChanged(
        Guid quoteId,
        Guid productId,
        int quantity)
    {
        QuoteId = quoteId;
        ProductId = productId;
        Quantity = quantity;
    }
}
