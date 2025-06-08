namespace EcommerceDDD.QuoteManagement.Application.Quotes.OpeningQuote;

public record class OpenQuote : ICommand
{
	public Currency Currency { get; private set; }

	public static OpenQuote Create(
		Currency currency)
	{
		if (currency is null)
			throw new ArgumentNullException(nameof(currency));

		return new OpenQuote(currency);
	}

	private OpenQuote(Currency currency)
	{
		Currency = currency;
	}
}