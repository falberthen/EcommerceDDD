namespace EcommerceDDD.OrderProcessing.Application.Payments.RequestingPayment;

public class RequestPaymentHandler(
	IOrderNotificationService orderNotificationService,
	IPaymentService paymentService,
	IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<RequestPayment>
{
	private readonly IOrderNotificationService _orderNotificationService = orderNotificationService
		?? throw new ArgumentNullException(nameof(orderNotificationService));
	private readonly IPaymentService _paymentService = paymentService
		?? throw new ArgumentNullException(nameof(paymentService));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));

	public async Task<Result> HandleAsync(RequestPayment command, CancellationToken cancellationToken)
	{
		Activity.Current?.SetTag("order.id", command.OrderId.Value.ToString());
		await Task.Delay(TimeSpan.FromSeconds(5));

		var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken);

		if (order is null)
			return Result.Fail($"Failed to find the order {command.OrderId}.");

		var productItems = order.OrderLines
			.Select(ol => new PaymentProductItem(
				ol.ProductItem.ProductId.Value,
				ol.ProductItem.ProductName,
				ol.ProductItem.Quantity,
				Convert.ToDouble(ol.ProductItem.UnitPrice.Amount)))
			.ToList();

		var paymentResult = await RequestPaymentAsync(command, productItems, cancellationToken);
		if (paymentResult.IsFailed)
			return paymentResult;

		try
		{
			await _orderNotificationService.UpdateOrderStatusAsync(
				order.CustomerId.Value,
				order.Id.Value,
				order.Status.ToString(),
				(int)order.Status,
				cancellationToken);
		}
		catch (Exception)
		{
			return Result.Fail($"An error occurred when updating status for order {order.Id.Value}.");
		}

		return Result.Ok();
	}

	private async Task<Result> RequestPaymentAsync(RequestPayment command, List<PaymentProductItem> productItems,
		CancellationToken cancellationToken)
	{
		try
		{
			await _paymentService.RequestPaymentAsync(
				command.CustomerId.Value,
				command.OrderId.Value,
				command.Currency.Code,
				Convert.ToDouble(command.TotalPrice.Amount),
				productItems,
				cancellationToken);

			return Result.Ok();
		}
		catch (Exception)
		{
			return Result.Fail($"An error occurred requesting payment for order {command.OrderId}.");
		}
	}
}
