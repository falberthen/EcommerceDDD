namespace EcommerceDDD.Quotes.Domain.Commands;

public record class RemoveQuoteItem : ICommand
{
    public QuoteId QuoteId { get; private set; }
    public ProductId ProductId { get; private set; }

    public static RemoveQuoteItem Create(
        QuoteId quoteId,
        ProductId productId)
    {
        if (quoteId is null)
            throw new ArgumentNullException(nameof(quoteId));
        if (productId is null)
            throw new ArgumentNullException(nameof(productId));

        return new RemoveQuoteItem(quoteId, productId);
    }

    private RemoveQuoteItem(QuoteId quoteId, ProductId productId)
    {
        QuoteId = quoteId;
        ProductId = productId;
    }
}