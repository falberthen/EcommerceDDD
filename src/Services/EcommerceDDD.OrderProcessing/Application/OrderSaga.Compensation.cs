namespace EcommerceDDD.OrderProcessing.Application;

/// <summary>
/// Handles failure/compensation events
/// </summary>
public partial class OrderSaga
{
	public CancelOrder Handle(CustomerReachedStoreCreditLimit @integrationEvent) =>
		CancelOrder.Create(
			OrderId.Of(@integrationEvent.OrderId),
			OrderCancellationReason.CustomerReachedStoreCreditLimit);

	public CancelOrder Handle(ShipmentNotDelivered @integrationEvent) =>
		CancelOrder.Create(
			OrderId.Of(@integrationEvent.OrderId),
			OrderCancellationReason.ShipmentNotDelivered);

	/// <summary>
	/// A canceled order ends the flow. If the order was already paid before cancellation,
	/// ask the payment service to cancel the payment on the way out.
	/// </summary>
	public RequestCancelPayment? Handle(OrderCanceled @domainEvent)
	{
		MarkCompleted();

		return @domainEvent.PaymentId is null
			? null
			: RequestCancelPayment.Create(
				OrderId.Of(@domainEvent.OrderId),
				PaymentId.Of(@domainEvent.PaymentId.Value),
				PaymentCancellationReason.OrderCanceled);
	}
}
