using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Quotes.Domain.Commands;

public record class CancelQuote : ICommand
{
    public QuoteId QuoteId { get; private set; }

    public static CancelQuote Create(
        QuoteId quoteId)
    {
        if (quoteId is null)
            throw new ArgumentNullException(nameof(quoteId));
    
        return new CancelQuote(quoteId);
    }

    private CancelQuote(QuoteId quoteId)
    {
        QuoteId = quoteId;
    }
}