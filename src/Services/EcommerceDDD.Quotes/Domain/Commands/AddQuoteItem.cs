using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Quotes.Domain.Commands;

public record class AddQuoteItem: ICommand
{
    public QuoteId QuoteId { get; private set; }
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Currency Currency { get; private set; }

    public static AddQuoteItem Create(
        QuoteId quoteId,
        ProductId productId,
        int quantity,
        Currency currency)
    {
        if (quoteId is null)
            throw new ArgumentNullException(nameof(quoteId));
        if (productId is null)
            throw new ArgumentNullException(nameof(productId));
        if (quantity == 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));
        if (currency is null)
            throw new ArgumentNullException(nameof(currency));

        return new AddQuoteItem(quoteId, productId, quantity, currency);
    }

    private AddQuoteItem(
        QuoteId quoteId,
        ProductId productId,
        int quantity,
        Currency currency)
    {
        QuoteId = quoteId;
        ProductId = productId;
        Quantity = quantity;
        Currency = currency;
    }
}