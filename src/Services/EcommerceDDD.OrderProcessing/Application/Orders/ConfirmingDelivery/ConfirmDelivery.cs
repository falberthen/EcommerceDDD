namespace EcommerceDDD.OrderProcessing.Application.Orders.ConfirmingDelivery;

public record class ConfirmDelivery : ICommand
{
	public OrderId OrderId { get; private set; }

	public static ConfirmDelivery Create(OrderId orderId)
	{
		if (orderId is null)
			throw new ArgumentNullException(nameof(orderId));

		return new ConfirmDelivery(orderId);
	}

	private ConfirmDelivery(OrderId orderId)
	{
		OrderId = orderId;
	}
}
