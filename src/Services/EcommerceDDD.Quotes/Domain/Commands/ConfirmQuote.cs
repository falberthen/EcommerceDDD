using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Quotes.Domain.Commands;

public record class ConfirmQuote : ICommand 
{    
    public QuoteId QuoteId { get; private set; }
    public Currency Currency { get; private set; }

    public static ConfirmQuote Create(
        QuoteId quoteId,
        Currency currency)
    {
        if (quoteId is null)
            throw new ArgumentNullException(nameof(quoteId));        
        if (currency is null)
            throw new ArgumentNullException(nameof(currency));

        return new ConfirmQuote(quoteId, currency);
    }
    private ConfirmQuote(
        QuoteId quoteId,
        Currency currency)
    {
        QuoteId = quoteId;
        Currency = currency;
    }
}