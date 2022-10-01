namespace EcommerceDDD.Core.Infrastructure.Kafka.Consumer;

public class KafkaConsumerConfig
{
    public string[]? Topics { get; set; }
    public string ConnectionString { get; set; }
    public string Group { get; set; }
}