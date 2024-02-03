namespace EcommerceDDD.QuoteManagement.Domain.Commands;

public record class AddQuoteItem: ICommand
{
    public QuoteId QuoteId { get; private set; }
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }

    public static AddQuoteItem Create(
        QuoteId quoteId,
        ProductId productId,
        int quantity)
    {
        if (quoteId is null)
            throw new ArgumentNullException(nameof(quoteId));
        if (productId is null)
            throw new ArgumentNullException(nameof(productId));
        if (quantity == 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        return new AddQuoteItem(quoteId, productId, quantity);
    }

    private AddQuoteItem(
        QuoteId quoteId,
        ProductId productId,
        int quantity)
    {
        QuoteId = quoteId;
        ProductId = productId;
        Quantity = quantity;
    }
}