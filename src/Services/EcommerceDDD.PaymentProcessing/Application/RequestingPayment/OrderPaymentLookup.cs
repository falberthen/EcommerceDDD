namespace EcommerceDDD.PaymentProcessing.Application.RequestingPayment;

public class OrderPaymentLookup(IQuerySession querySession) : IOrderPaymentLookup
{
	private readonly IQuerySession _querySession = querySession
		?? throw new ArgumentNullException(nameof(querySession));

	public Task<bool> HasPaymentAsync(OrderId orderId, CancellationToken cancellationToken) =>
		_querySession.Query<PaymentDetails>()
			.AnyAsync(d => d.OrderId == orderId.Value, cancellationToken);
}
