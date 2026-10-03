namespace EcommerceDDD.PaymentProcessing.Application.RequestingPayment;

/// <summary>
/// Tells whether a payment was already requested for an order.
/// </summary>
public interface IOrderPaymentLookup
{
	Task<bool> HasPaymentAsync(OrderId orderId, CancellationToken cancellationToken);
}
