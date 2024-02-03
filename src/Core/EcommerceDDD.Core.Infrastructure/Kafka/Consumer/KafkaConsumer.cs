namespace EcommerceDDD.Core.Infrastructure.Kafka.Consumer;

public class KafkaConsumer : IEventConsumer
{
    private readonly IEventPublisher _eventPublisher;
    private readonly IConsumer<string, INotification> _consumer;
    private readonly ILogger<KafkaConsumer> _logger;

    public KafkaConsumer(
        IEventPublisher eventPublisher,
        JsonEventSerializer<INotification> serializer,
        IOptions<KafkaConsumerConfig> kafkaConsumerConfig,
        ILogger<KafkaConsumer> logger)
    {
        _logger = logger;
        _eventPublisher = eventPublisher;

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
        
        try
        {
            _consumer = new ConsumerBuilder<string, INotification>(consumerConfig)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(serializer)
                .Build();

            _logger.LogInformation("Subscribing to topics: {Topics}", string.Join(", ", config.Topics));
            _consumer.Subscribe(config.Topics);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error subscribing to Kafka topics: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    public async Task StartConsumeAsync(CancellationToken cancellationToken = default)
    {
        if (_consumer == null)
        {
            _logger.LogError("Kafka consumer is not initialized. Unable to start consumption.");
            return;
        }

        _logger.LogInformation("Kafka consumer started.");
        
        while (!cancellationToken.IsCancellationRequested)
            await ConsumeNextMessage(_consumer, cancellationToken);        
    }

    private async Task ConsumeNextMessage(IConsumer<string, INotification> consumer, 
        CancellationToken cancellationToken)
    {
        var retryPolicy = Policy
            .Handle<KafkaException>()
            .WaitAndRetryForeverAsync(attempt => TimeSpan.FromSeconds(5));

        var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromHours(1));
        var policyWrap = Policy.WrapAsync(timeoutPolicy, retryPolicy);

        await policyWrap.ExecuteAsync(async () =>
        {
            await Task.Yield();
            var @event = _consumer.Consume(cancellationToken);

            if (@event is null)
            {
                _logger.LogError($"Unable to deserialize integration event: {@event}");
                await Task.CompletedTask;
            }

            _logger.LogInformation($"Dispatching event: {@event}");
            await _eventPublisher.PublishEventAsync(@event!.Message.Value, 
                cancellationToken);
            consumer.Commit();
        })
        .ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                var exception = task.Exception?.Flatten().InnerException;
                _logger.LogError("An error occurred when consuming a Kafka message: {Message} {StackTrace}",
                    exception?.Message, exception?.StackTrace);
                _consumer.Close();
            }
        });
    }
}
