namespace EcommerceDDD.PaymentProcessing.Application.ProcessingPayment;

/// <summary>
/// Processes a payment once it is created.
/// </summary>
public static class PaymentCreatedHandler
{
	public static ProcessPayment Handle(PaymentCreated @event) =>
		ProcessPayment.Create(PaymentId.Of(@event.PaymentId), OrderId.Of(@event.OrderId));
}
