namespace EcommerceDDD.QuoteManagement.Domain.Events;

public record class QuoteItemAdded : DomainEvent
{
    public Guid QuoteId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal ProductPrice { get; private set; }
    public int Quantity { get; private set; }

    public static QuoteItemAdded Create(QuoteItemData quoteItemData) 
    {
        if (quoteItemData.QuoteId is null)
            throw new ArgumentNullException(nameof(QuoteId));
        if (quoteItemData.ProductId.Value == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(quoteItemData.ProductId));
        if (string.IsNullOrEmpty(quoteItemData.ProductName))
            throw new ArgumentOutOfRangeException(nameof(quoteItemData.ProductId));
        if (quoteItemData.Quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quoteItemData.Quantity));

        return new QuoteItemAdded(
            quoteItemData.QuoteId.Value,
            quoteItemData.ProductId.Value,
            quoteItemData.ProductName,
            quoteItemData.ProductPrice.Amount,
            quoteItemData.Quantity);
    }

    private QuoteItemAdded(
        Guid quoteId,
        Guid productId,
        string productName,
        decimal productPrice,
        int quantity)
    {
        QuoteId = quoteId;
        ProductId = productId;
        ProductName = productName;
        ProductPrice = productPrice;
        Quantity = quantity;
    }
}
