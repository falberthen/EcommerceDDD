namespace EcommerceDDD.Core.Infrastructure.OpenTelemetry;

/// <summary>
/// Project-specific telemetry tag keys and literal values that are not part of
/// the OpenTelemetry semantic conventions spec. Keys defined by the spec live
/// in <see cref="MessagingAttributes"/> (and future domain attribute files).
/// </summary>
public static class TelemetryTags
{
	public const string OrderId   = "order.id";
	public const string EventType = "event.type";

	public static class DestinationValues
	{
		public const string Outbox = "outbox";
	}
}
