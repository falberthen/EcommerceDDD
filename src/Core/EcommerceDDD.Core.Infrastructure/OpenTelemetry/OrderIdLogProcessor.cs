namespace EcommerceDDD.Core.Infrastructure.OpenTelemetry;

/// <summary>
/// Adds the "order.id" of the active span, or of its in-process parents, to each log record.
/// </summary>
public class OrderIdLogProcessor : BaseProcessor<LogRecord>
{
	public override void OnEnd(LogRecord record)
	{
		var orderId = FindOrderId(Activity.Current);
		if (orderId is null)
			return;

		var attributes = record.Attributes?.ToList() ?? [];
		if (attributes.Any(attribute => attribute.Key == OrderIdTagPolicy.OrderIdTag))
			return;

		attributes.Add(new(OrderIdTagPolicy.OrderIdTag, orderId));
		record.Attributes = attributes;
	}

	private static string? FindOrderId(Activity? activity)
	{
		for (; activity is not null; activity = activity.Parent)
		{
			if (activity.GetTagItem(OrderIdTagPolicy.OrderIdTag) is { } orderId)
				return orderId.ToString();
		}

		return null;
	}
}
