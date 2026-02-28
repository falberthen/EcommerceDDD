namespace EcommerceDDD.OrderProcessing.Application;

public class OrderSaga(
	ICommandBus commandBus
) : IEventHandler<OrderPlaced>,
	IEventHandler<OrderProcessed>,
	IEventHandler<PaymentFinalized>,
	IEventHandler<ShipmentFinalized>
{
	private readonly ICommandBus _commandBus = commandBus;

	/// <summary>
	/// Processing placed order
	/// </summary>
	public async Task HandleAsync(OrderPlaced @domainEvent,
		CancellationToken cancellationToken)
	{
		var processOrderCommand = ProcessOrder.Create(
			CustomerId.Of(@domainEvent.CustomerId),
			OrderId.Of(@domainEvent.OrderId),
			QuoteId.Of(@domainEvent.QuoteId)
		);

		var result = await _commandBus
			.SendAsync(processOrderCommand, cancellationToken);
		ThrowIfFailed(result);
	}

	/// <summary>
	/// Requesting payment
	/// </summary>
	public async Task HandleAsync(OrderProcessed @domainEvent,
		CancellationToken cancellationToken)
	{
		var requestPaymentCommand = RequestPayment.Create(
			CustomerId.Of(@domainEvent.CustomerId),
			OrderId.Of(@domainEvent.OrderId),
			Money.Of(@domainEvent.TotalPrice, @domainEvent.CurrencyCode),
			Currency.OfCode(@domainEvent.CurrencyCode));

		var result = await _commandBus
			.SendAsync(requestPaymentCommand, cancellationToken);
		ThrowIfFailed(result);
	}

	/// <summary>
	/// Requesting shipment — RecordPayment is idempotent, so retries are safe
	/// </summary>
	public async Task HandleAsync(PaymentFinalized @integrationEvent,
		CancellationToken cancellationToken)
	{
		var recordPaymentCommand = RecordPayment.Create(
			OrderId.Of(@integrationEvent.OrderId),
			PaymentId.Of(@integrationEvent.PaymentId),
			Money.Of(@integrationEvent.TotalAmount,
				@integrationEvent.CurrencyCode));

		var recordResult = await _commandBus
			.SendAsync(recordPaymentCommand, cancellationToken);
		ThrowIfFailed(recordResult);

		var requestShipmentCommand = RequestShipment.Create(
			OrderId.Of(@integrationEvent.OrderId));

		var shipmentResult = await _commandBus
			.SendAsync(requestShipmentCommand, cancellationToken);
		ThrowIfFailed(shipmentResult);
	}

	/// <summary>
	/// Recording shipment — order awaits customer delivery confirmation
	/// </summary>
	public async Task HandleAsync(ShipmentFinalized @integrationEvent,
		CancellationToken cancellationToken)
	{
		var recordShipmentCommand = RecordShipment.Create(
			OrderId.Of(@integrationEvent.OrderId),
			ShipmentId.Of(@integrationEvent.ShipmentId));

		var result = await _commandBus
			.SendAsync(recordShipmentCommand, cancellationToken);
		ThrowIfFailed(result);
	}

	private static void ThrowIfFailed(Result result)
	{
		if (result.IsFailed)
			throw new InvalidOperationException(
				result.Errors.FirstOrDefault()?.Message ?? "Saga step failed.");
	}
}
