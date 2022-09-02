global using MediatR;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EcommerceDDD.Core.Infrastructure.Kafka.Events;
using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Core.Infrastructure.Kafka.Consumer;

public class KafkaConsumer : IEventConsumer
{
    private readonly ILogger<KafkaConsumer> _logger;
    private IConsumer<string, string> _consumer;
    private IMediator _mediator;

    public KafkaConsumer(
        IOptions<KafkaConsumerConfig> kafkaConsumerConfig,
        ILogger<KafkaConsumer> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;

        if (kafkaConsumerConfig == null)
            throw new ArgumentNullException(nameof(kafkaConsumerConfig));

        var config = kafkaConsumerConfig.Value;
        var consumerConfig = new ConsumerConfig
        {
            GroupId = config.Group,
            BootstrapServers = config.ConnectionString,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        var consumerBuilder = new ConsumerBuilder<string, string>(consumerConfig);
        _consumer = consumerBuilder.Build();
        _consumer.Subscribe(config.Topics);
    }

    public async Task StartConsumeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Kafka consumer started");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
                await ConsumeNextEvent(_consumer, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("An error occurred when consuming a Kafka message: {Message} {StackTrace}", e.Message, e.StackTrace);
            _consumer.Close();
        }
    }

    private async Task ConsumeNextEvent(IConsumer<string, string> consumer, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Yield();
            var result = consumer.Consume(cancellationToken);

            var @event = result.ToIntegrationEvent();

            if(@event != null)
            {
                await _mediator.Publish(@event, cancellationToken);
                consumer.Commit();
            }
            else                
                _logger.LogError("Unable to deserialize integration event.", result);
        }
        catch (Exception e)
        {
            _logger.LogError("An error occurred when producing a Kafka message: {Message} {StackTrace}", e.Message, e.StackTrace);
        }
    }
}
