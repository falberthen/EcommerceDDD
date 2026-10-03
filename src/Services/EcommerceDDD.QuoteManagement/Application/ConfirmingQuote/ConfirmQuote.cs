namespace EcommerceDDD.QuoteManagement.Application.ConfirmingQuote;

public record class ConfirmQuote : ICommand
{
	public QuoteId QuoteId { get; private set; }
	public OrderId OrderId { get; private set; }

	public static ConfirmQuote Create(
		QuoteId quoteId,
		OrderId orderId)
	{
		if (quoteId is null)
			throw new ArgumentNullException(nameof(quoteId));
		if (orderId is null)
			throw new ArgumentNullException(nameof(orderId));

		return new ConfirmQuote(quoteId, orderId);
	}

	private ConfirmQuote(QuoteId quoteId, OrderId orderId)
	{
		QuoteId = quoteId;
		OrderId = orderId;
	}
}