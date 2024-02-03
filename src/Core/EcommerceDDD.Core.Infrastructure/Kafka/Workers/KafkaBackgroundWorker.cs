namespace EcommerceDDD.Core.Infrastructure.Kafka.Workers;

public class KafkaBackgroundWorker : BackgroundWorker
{
    public KafkaBackgroundWorker(ILogger<KafkaBackgroundWorker> logger, IEventConsumer consumer)
        : base(logger, consumer.StartConsumeAsync)
    {
    }
}
