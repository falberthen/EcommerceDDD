namespace EcommerceDDD.QuoteManagement.Application.Quotes.GettingQuoteById;

public record class GetQuoteById : IQuery<QuoteViewModel>
{
	public QuoteId QuoteId { get; private set; }

	public static GetQuoteById Create(
		QuoteId quoteId)
	{
		if (quoteId is null)
			throw new ArgumentNullException(nameof(quoteId));

		return new GetQuoteById(quoteId);
	}

	private GetQuoteById(
		QuoteId quoteId)
	{
		QuoteId = quoteId;
	}
}