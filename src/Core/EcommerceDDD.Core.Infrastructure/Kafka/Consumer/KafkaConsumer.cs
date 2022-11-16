using Polly;
using Confluent.Kafka;
using EcommerceDDD.Core.EventBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EcommerceDDD.Core.Infrastructure.Kafka.Serialization;

namespace EcommerceDDD.Core.Infrastructure.Kafka.Consumer;

public class KafkaConsumer : IEventConsumer
{
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IConsumer<string, IntegrationEvent> _consumer;
    private readonly ILogger<KafkaConsumer> _logger;

    public KafkaConsumer(
        IEventDispatcher eventDispatcher,
        JsonEventSerializer<IntegrationEvent> serializer,
        IOptions<KafkaConsumerConfig> kafkaConsumerConfig,
        ILogger<KafkaConsumer> logger)
    {
        _logger = logger;
        _eventDispatcher = eventDispatcher;

        if (kafkaConsumerConfig is null)
            throw new ArgumentNullException(nameof(kafkaConsumerConfig));

        var config = kafkaConsumerConfig.Value;
        var consumerConfig = new ConsumerConfig
        {
            GroupId = config.Group,
            BootstrapServers = config.ConnectionString,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
        
        _consumer =
            new ConsumerBuilder<string, IntegrationEvent>(consumerConfig)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(serializer)
            .Build();

        _consumer.Subscribe(config.Topics);
    }

    public async Task StartConsumeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Kafka consumer started.");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
                await ConsumeNextMessage(_consumer, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("An error occurred when consuming a Kafka message: {Message} {StackTrace}", e.Message, e.StackTrace);
            _consumer.Close();
        }
    }

    private async Task ConsumeNextMessage(IConsumer<string, IntegrationEvent> consumer, CancellationToken cancellationToken)
    {
        var policy = Policy
           .Handle<Exception>()
           .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        await policy.ExecuteAsync(
           async () =>
           {
               await Task.Yield();
               var @event = _consumer.Consume(cancellationToken);

               if (@event is null)
               {
                   _logger.LogError("Unable to deserialize integration event.", consumer);
                   await Task.CompletedTask;
               }

               _logger.LogInformation("Dispatching event: {event}", @event);

               await _eventDispatcher.DispatchAsync(@event!.Message.Value);
               consumer.Commit();
           });
    }
}
