namespace EcommerceDDD.OrderProcessing.Application;

/// <summary>
/// Handles failure/compensation events
/// </summary>
public partial class OrderSaga
{
	public CancelOrder Handle(PaymentFailed @integrationEvent) =>
		CancelOrder.Create(
			OrderId.Of(@integrationEvent.OrderId),
			OrderCancellationReason.PaymentFailed);

	public CancelOrder Handle(CustomerReachedCreditLimit @integrationEvent) =>
		CancelOrder.Create(
			OrderId.Of(@integrationEvent.OrderId),
			OrderCancellationReason.CustomerReachedCreditLimit);

	public CancelOrder Handle(ShipmentFailed @integrationEvent) =>
		CancelOrder.Create(
			OrderId.Of(@integrationEvent.OrderId),
			OrderCancellationReason.ShipmentFailed);

	public CancelOrder Handle(ProductWasOutOfStock @integrationEvent) =>
		CancelOrder.Create(
			OrderId.Of(@integrationEvent.OrderId),
			OrderCancellationReason.ProductWasOutOfStock);

	/// <summary>
	/// If the order was already paid before cancellation, ask the payment service to cancel the payment.
	/// </summary>
	public RequestCancelPayment? Handle(OrderCanceled @domainEvent) =>
		@domainEvent.PaymentId is null
			? null
			: RequestCancelPayment.Create(
				OrderId.Of(@domainEvent.OrderId),
				PaymentId.Of(@domainEvent.PaymentId.Value),
				PaymentCancellationReason.OrderCanceled);
}
