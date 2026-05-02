// Local subset of OpenTelemetry messaging semantic conventions.
// Todo: replace constant with the official `OpenTelemetry.SemanticConventions` package is released.
// Source: https://github.com/open-telemetry/opentelemetry-dotnet-contrib/blob/main/src/OpenTelemetry.SemanticConventions/Attributes/MessagingAttributes.cs

namespace EcommerceDDD.Core.Infrastructure.OpenTelemetry;

public static class MessagingAttributes
{
	/// <summary>An identifier for the messaging system being used. See <c>MessagingSystemValues</c>.</summary>
	public const string AttributeMessagingSystem = "messaging.system";

	/// <summary>The message destination name (queue, topic, or broker-specific entity).</summary>
	public const string AttributeMessagingDestinationName = "messaging.destination.name";

	/// <summary>The partition identifier within <c>messaging.destination.name</c>.</summary>
	public const string AttributeMessagingDestinationPartitionId = "messaging.destination.partition.id";

	/// <summary>The type of the messaging operation. See <c>MessagingOperationTypeValues</c>.</summary>
	public const string AttributeMessagingOperationType = "messaging.operation.type";

	/// <summary>The name of the consumer group with which a consumer is associated.</summary>
	public const string AttributeMessagingConsumerGroupName = "messaging.consumer.group.name";

	/// <summary>The offset of a record in the corresponding Kafka partition.</summary>
	public const string AttributeMessagingKafkaOffset = "messaging.kafka.message.offset";

	/// <summary>The Kafka message key (partition routing key; not a unique message id).</summary>
	public const string AttributeMessagingKafkaMessageKey = "messaging.kafka.message.key";

	/// <summary>A value used by the messaging system as an identifier for the message.</summary>
	public const string AttributeMessagingMessageId = "messaging.message.id";

	public static class MessagingSystemValues
	{
		public const string Kafka = "kafka";
	}

	public static class MessagingOperationTypeValues
	{
		/// <summary>One or more messages are provided for sending to an intermediary.</summary>
		public const string Send = "send";

		/// <summary>One or more messages are requested by a consumer (pull-based).</summary>
		public const string Receive = "receive";

		/// <summary>One or more messages are processed by a consumer.</summary>
		public const string Process = "process";
	}
}
