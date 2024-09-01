namespace EcommerceDDD.Core.Infrastructure.Kafka.Workers;

public class KafkaBackgroundWorker : Infrastructure.Workers.BackgroundWorker
{
	public KafkaBackgroundWorker(ILogger<KafkaBackgroundWorker> logger, IEventConsumer consumer)
		: base(logger, consumer.StartConsumeAsync) { }
}
