namespace EcommerceDDD.OrderProcessing.Application.Orders.PlacingOrder;

public record class PlaceOrder : ICommand
{
	[Audit]
	public OrderId OrderId { get; private set; }
	public QuoteId QuoteId { get; private set; }

	public static PlaceOrder Create(
		OrderId orderId,
		QuoteId quoteId)
	{
		if (orderId is null)
			throw new ArgumentNullException(nameof(orderId));
		if (quoteId is null)
			throw new ArgumentNullException(nameof(quoteId));

		return new PlaceOrder(orderId, quoteId);
	}

	private PlaceOrder(
		OrderId orderId,
		QuoteId quoteId)
	{
		OrderId = orderId;
		QuoteId = quoteId;
	}
}