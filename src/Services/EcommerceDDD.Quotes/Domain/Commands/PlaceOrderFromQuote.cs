using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Quotes.Domain.Commands;

public record class PlaceOrderFromQuote : ICommand 
{    
    public QuoteId QuoteId { get; private set; }
    public Currency Currency { get; private set; }

    public static PlaceOrderFromQuote Create(
        QuoteId quoteId,
        Currency currency)
    {
        if (quoteId is null)
            throw new ArgumentNullException(nameof(quoteId));        
        if (currency is null)
            throw new ArgumentNullException(nameof(currency));

        return new PlaceOrderFromQuote(quoteId, currency);
    }
    private PlaceOrderFromQuote(
        QuoteId quoteId,
        Currency currency)
    {
        QuoteId = quoteId;
        Currency = currency;
    }
}