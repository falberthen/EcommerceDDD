using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.CQRS.QueryHandling;

namespace EcommerceDDD.Quotes.Application.Quotes.GettingConfirmedQuote;

public record class GetConfirmedQuoteById : IQuery<QuoteViewModel>
{
    public QuoteId QuoteId { get; private set; }

    public static GetConfirmedQuoteById Create(
        QuoteId quoteId)
    {
        if (quoteId is null)
            throw new ArgumentNullException(nameof(quoteId));
        
        return new GetConfirmedQuoteById(quoteId);
    }

    private GetConfirmedQuoteById(
        QuoteId quoteId)
    {
        QuoteId = quoteId;
    }
}