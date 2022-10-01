using Confluent.Kafka;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Core.Infrastructure.Kafka.Producer;

public class KafkaProducer : IEventProducer
{
    private readonly ILogger<KafkaProducer> _logger;
    private readonly KafkaProducerConfig _kafkaConfig;
    private IProducer<string, string> _producer;

    public KafkaProducer(IOptions<KafkaProducerConfig> producerConfigOptions, ILogger<KafkaProducer> logger)
    {
        _logger = logger;

        if (producerConfigOptions is null)
            throw new ArgumentNullException(nameof(producerConfigOptions));

        _kafkaConfig = producerConfigOptions.Value;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaConfig.ConnectionString,
        };

        var producerBuilder = new ProducerBuilder<string, string>(consumerConfig);
        _producer = producerBuilder.Build();
    }

    public async Task PublishAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            await _producer.ProduceAsync(_kafkaConfig.Topic,
                new Message<string, string>
                {
                    Key = @event.GetType().Name,
                    Value = JsonConvert.SerializeObject(@event)
                }, 
                cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.LogError("Unable to produce Kafka message: {Message} {StackTrace}", e.Message, e.StackTrace);
            throw;
        }
    }
}
