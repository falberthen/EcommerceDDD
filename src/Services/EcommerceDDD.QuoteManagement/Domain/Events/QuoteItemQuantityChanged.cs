namespace EcommerceDDD.QuoteManagement.Domain.Events;

public record class QuoteItemQuantityChanged : DomainEvent
{
    public Guid QuoteId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }

    public static QuoteItemQuantityChanged Create(QuoteItemData quoteItemData)
    {
        if (quoteItemData.QuoteId is null)
            throw new ArgumentNullException(nameof(QuoteId));
        if (quoteItemData.ProductId is null)
            throw new ArgumentNullException(nameof(ProductId));                
        if (quoteItemData.Quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(Quantity));

        return new QuoteItemQuantityChanged(
            quoteItemData.QuoteId.Value,
            quoteItemData.ProductId.Value,
            quoteItemData.Quantity);
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
