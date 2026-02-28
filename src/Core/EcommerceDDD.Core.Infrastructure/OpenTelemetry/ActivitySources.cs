namespace EcommerceDDD.Core.Infrastructure.OpenTelemetry;

public static class ActivitySources
{
	public const string CommandBus    = "EcommerceDDD.CommandBus";
	public const string KafkaConsumer = "EcommerceDDD.Kafka.Consumer";
	public const string OutboxWrite   = "EcommerceDDD.Outbox.Write";
}
