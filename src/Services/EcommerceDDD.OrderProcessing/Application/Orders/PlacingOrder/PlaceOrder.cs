namespace EcommerceDDD.OrderProcessing.Application.Orders.PlacingOrder;

public record class PlaceOrder : ICommand
{
	public QuoteId QuoteId { get; private set; }

	public static PlaceOrder Create(
		QuoteId quoteId)
	{
		if (quoteId is null)
			throw new ArgumentNullException(nameof(quoteId));

		return new PlaceOrder(quoteId);
	}

	private PlaceOrder(
		QuoteId quoteId)
	{
		QuoteId = quoteId;
	}
}