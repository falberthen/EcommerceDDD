namespace EcommerceDDD.OrderProcessing.Infrastructure.OpenTelemetry;

/// <summary>
/// Counts the orders that reach a terminal state, by outcome and cancellation reason.
/// </summary>
public class OrderMetrics
{
	// Named after the service, which is the meter AddOpenTelemetryObservability exports.
	public static readonly string MeterName = typeof(OrderMetrics).Assembly.GetName().Name!;
	public const string OrdersCounterName = "ecommerceddd.orders";

	private readonly Counter<long> _orders;

	public OrderMetrics(IMeterFactory meterFactory) =>
		_orders = meterFactory.Create(MeterName).CreateCounter<long>(
			OrdersCounterName, unit: "{order}", description: "Orders that reached a terminal state.");

	public void RecordCompleted() =>
		_orders.Add(1, new KeyValuePair<string, object?>("outcome", "completed"));

	public void RecordCanceled(OrderCancellationReason reason) =>
		_orders.Add(1,
			new KeyValuePair<string, object?>("outcome", "canceled"),
			new KeyValuePair<string, object?>("reason", reason switch
			{
				OrderCancellationReason.ProductWasOutOfStock => "out_of_stock",
				OrderCancellationReason.CustomerReachedStoreCreditLimit => "credit_limit",
				OrderCancellationReason.ShipmentNotDelivered => "not_delivered",
				_ => "other"
			}));
}
