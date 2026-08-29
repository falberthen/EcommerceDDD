namespace EcommerceDDD.OrderProcessing.Application;

/// <summary>
/// Coordinates the order fulfilment.
/// </summary>
public partial class OrderSaga : Saga
{
	/// <summary>
	/// Saga identity. It correlates to every OrderId in the messages flow.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Processing placed order
	/// </summary>
	public static (OrderSaga, ProcessOrder) Start(OrderPlaced @domainEvent) =>
	(
		new OrderSaga { Id = @domainEvent.OrderId },
		ProcessOrder.Create(
			CustomerId.Of(@domainEvent.CustomerId),
			OrderId.Of(@domainEvent.OrderId),
			QuoteId.Of(@domainEvent.QuoteId))
	);

	/// <summary>
	/// Requesting payment
	/// </summary>
	public RequestPayment Handle(OrderProcessed @domainEvent) =>
		RequestPayment.Create(
			CustomerId.Of(@domainEvent.CustomerId),
			OrderId.Of(@domainEvent.OrderId),
			Money.Of(@domainEvent.TotalPrice, @domainEvent.CurrencyCode),
			Currency.OfCode(@domainEvent.CurrencyCode));

	/// <summary>
	/// Recording payment
	/// </summary>
	public RecordPayment Handle(PaymentFinalized @integrationEvent) =>
		RecordPayment.Create(
			OrderId.Of(@integrationEvent.OrderId),
			PaymentId.Of(@integrationEvent.PaymentId),
			Money.Of(@integrationEvent.TotalAmount, @integrationEvent.CurrencyCode));

	/// <summary>
	/// Requesting shipment once the payment is recorded on the order
	/// </summary>
	public RequestShipment Handle(OrderPaid @domainEvent) =>
		RequestShipment.Create(OrderId.Of(@domainEvent.OrderId));

	/// <summary>
	/// Recording shipment. Order awaits customer for delivery confirmation
	/// </summary>
	public RecordShipment Handle(ShipmentFinalized @integrationEvent) =>
		RecordShipment.Create(
			OrderId.Of(@integrationEvent.OrderId),
			ShipmentId.Of(@integrationEvent.ShipmentId));
}
