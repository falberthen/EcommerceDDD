namespace EcommerceDDD.QuoteManagement.Application.Quotes.CancelingQuote;

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

	private CancelQuote(QuoteId quoteId) => QuoteId = quoteId;
}