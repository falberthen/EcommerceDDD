namespace EcommerceDDD.OrderProcessing.Application;

/// <summary>
/// Handles failure/compensation events
/// </summary>
public partial class OrderSaga :
	IEventHandler<PaymentFailed>,
	IEventHandler<CustomerReachedCreditLimit>,
	IEventHandler<ShipmentFailed>,
	IEventHandler<ProductWasOutOfStock>,
	IEventHandler<OrderCanceled>
{
	public async Task HandleAsync(PaymentFailed @integrationEvent,
		CancellationToken cancellationToken)
	{
		var command = CancelOrder.Create(
			OrderId.Of(@integrationEvent.OrderId),
			OrderCancellationReason.PaymentFailed);

		var result = await _commandBus.SendAsync(command, cancellationToken);
		ThrowIfFailed(result);
	}

	public async Task HandleAsync(CustomerReachedCreditLimit @integrationEvent,
		CancellationToken cancellationToken)
	{
		var command = CancelOrder.Create(
			OrderId.Of(@integrationEvent.OrderId),
			OrderCancellationReason.CustomerReachedCreditLimit);

		var result = await _commandBus.SendAsync(command, cancellationToken);
		ThrowIfFailed(result);
	}

	public async Task HandleAsync(ShipmentFailed @integrationEvent,
		CancellationToken cancellationToken)
	{
		var command = CancelOrder.Create(
			OrderId.Of(@integrationEvent.OrderId),
			OrderCancellationReason.ShipmentFailed);

		var result = await _commandBus.SendAsync(command, cancellationToken);
		ThrowIfFailed(result);
	}

	public async Task HandleAsync(ProductWasOutOfStock @integrationEvent,
		CancellationToken cancellationToken)
	{
		var command = CancelOrder.Create(
			OrderId.Of(@integrationEvent.OrderId),
			OrderCancellationReason.ProductWasOutOfStock);

		var result = await _commandBus.SendAsync(command, cancellationToken);
		ThrowIfFailed(result);
	}

	public async Task HandleAsync(OrderCanceled @integrationEvent,
		CancellationToken cancellationToken)
	{
		// If the order was already paid before cancellation, ask payment service to cancel the payment
		if (!@integrationEvent.PaymentId.HasValue)
			return;

		var command = RequestCancelPayment.Create(
			OrderId.Of(@integrationEvent.OrderId),
			PaymentId.Of(@integrationEvent.PaymentId.Value),
			PaymentCancellationReason.OrderCanceled);

		var result = await _commandBus.SendAsync(command, cancellationToken);
		ThrowIfFailed(result);
	}
}
