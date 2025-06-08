namespace EcommerceDDD.QuoteManagement.Application.Quotes.ConfirmingQuote;

public record class ConfirmQuote : ICommand
{
	public QuoteId QuoteId { get; private set; }

	public static ConfirmQuote Create(
		QuoteId quoteId)
	{
		if (quoteId is null)
			throw new ArgumentNullException(nameof(quoteId));

		return new ConfirmQuote(quoteId);
	}
	private ConfirmQuote(QuoteId quoteId) => QuoteId = quoteId;
}