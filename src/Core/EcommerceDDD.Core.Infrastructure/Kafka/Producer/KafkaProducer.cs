global using MediatR;
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

    public async Task PublishAsync(INotification @event, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new Message<string, string>
            {
                Key = @event.GetType().Name,
                Value = JsonConvert.SerializeObject(@event)
            };

            _logger.LogInformation("Publishing message {message} to topic {Topic}...", message, _kafkaConfig.Topic);
            await _producer.ProduceAsync(_kafkaConfig.Topic, message, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.LogError("Unable to produce Kafka message: {Message} {StackTrace}", e.Message, e.StackTrace);
            throw;
        }
    }
}
