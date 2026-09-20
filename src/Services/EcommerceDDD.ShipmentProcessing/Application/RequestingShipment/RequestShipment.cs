namespace EcommerceDDD.ShipmentProcessing.Application.RequestingShipment;

public record class RequestShipment : ICommand
{
	[Audit]
	public OrderId OrderId { get; private set; }
	public Guid CustomerId { get; private set; }
	public IReadOnlyList<ProductItem> ProductItems { get; private set; }

	public static RequestShipment Create(
		OrderId orderId,
		Guid customerId,
		IReadOnlyList<ProductItem> productItems)
	{
		if (orderId is null)
			throw new ArgumentNullException(nameof(OrderId));
		if (customerId == Guid.Empty)
			throw new ArgumentException("The customer id is required.", nameof(customerId));
		if (productItems.Count == 0)
			throw new ArgumentOutOfRangeException(nameof(productItems));

		return new RequestShipment(orderId, customerId, productItems);
	}

	private RequestShipment(
		OrderId orderId,
		Guid customerId,
		IReadOnlyList<ProductItem> productItems)
	{
		OrderId = orderId;
		CustomerId = customerId;
		ProductItems = productItems;
	}
}