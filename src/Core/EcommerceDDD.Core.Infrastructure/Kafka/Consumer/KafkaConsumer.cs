using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace EcommerceDDD.Core.Infrastructure.Kafka.Consumer;

public class KafkaConsumer : IEventConsumer
{
	private readonly IEventBus _eventPublisher;
	private readonly ILogger<KafkaConsumer> _logger;
	private readonly KafkaConsumerConfig _config;
	private readonly JsonEventSerializer<INotification> _serializer;
	private IConsumer<string, INotification>? _consumer;
	private bool _isInitialized = false;

	public KafkaConsumer(
		IEventBus eventPublisher,
		JsonEventSerializer<INotification> serializer,
		IOptions<KafkaConsumerConfig> kafkaConsumerConfig,
		ILogger<KafkaConsumer> logger)
	{
		_logger = logger;
		_eventPublisher = eventPublisher;
		_serializer = serializer;

		_config = kafkaConsumerConfig?.Value
			?? throw new ArgumentNullException(nameof(kafkaConsumerConfig));

		if (_config.Topics is null || !_config.Topics.Any())
			throw new ArgumentNullException(nameof(_config.Topics), "Kafka topics must be configured.");
	}

	public async Task StartConsumeAsync(CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Starting Kafka consumer...");

		try
		{
			await InitializeConsumerWithRetryAsync(cancellationToken);

			_logger.LogInformation("Kafka consumer started successfully.");

			while (!cancellationToken.IsCancellationRequested)
			{
				if (!_isInitialized)
				{
					_logger.LogWarning("Consumer not initialized. Attempting to reinitialize...");
					await InitializeConsumerWithRetryAsync(cancellationToken);
					continue;
				}

				await ConsumeNextMessage(_consumer!, cancellationToken);
			}
		}
		catch (OperationCanceledException)
		{
			_logger.LogInformation("Kafka consumer cancellation requested.");
		}
		catch (ObjectDisposedException)
		{
			_logger.LogWarning("Kafka consumer was disposed during operation.");
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, "Unhandled exception in Kafka consumer.");
			throw;
		}
		finally
		{
			try
			{
				_consumer?.Close();
			}
			catch (ObjectDisposedException)
			{
				_logger.LogWarning("Kafka consumer already disposed before close.");
			}

			_consumer?.Dispose();
			_logger.LogInformation("Kafka consumer stopped.");
		}
	}

	private async Task InitializeConsumerWithRetryAsync(CancellationToken cancellationToken)
	{
		if (_isInitialized)
			return;

		var retryPolicy = Policy
			.Handle<KafkaException>()
			.Or<InvalidOperationException>()
			.WaitAndRetryAsync(
				retryCount: 10,
				sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Min(Math.Pow(2, retryAttempt), 30)),
				onRetry: (exception, timeSpan, retryCount, context) =>
				{
					_logger.LogWarning("Retry {RetryCount} after Kafka init error: {Message}. Retrying in {Delay}s.",
						retryCount, exception.Message, timeSpan.TotalSeconds);
				});

		await retryPolicy.ExecuteAsync(async () =>
		{
			var consumerConfig = new ConsumerConfig
			{
				GroupId = _config.Group,
				BootstrapServers = _config.ConnectionString,
				AutoOffsetReset = AutoOffsetReset.Earliest,
				EnableAutoCommit = false,
				AllowAutoCreateTopics = false,
				MaxPollIntervalMs = 300000,
				SessionTimeoutMs = 10000,
				EnablePartitionEof = true
			};

			_consumer?.Dispose();
			_consumer = new ConsumerBuilder<string, INotification>(consumerConfig)
				.SetKeyDeserializer(Deserializers.Utf8)
				.SetValueDeserializer(_serializer)
				.SetErrorHandler((_, error) =>
					_logger.LogError("Kafka error: {Reason} (Code: {Code})", error.Reason, error.Code))
				.SetLogHandler((_, message) =>
					_logger.LogDebug("Kafka: {Message} (Level: {Level})", message.Message, message.Level))
				.Build();
			
			await EnsureTopicsExistAsync();

			_logger.LogInformation("Subscribing to topics: {Topics}", string.Join(", ", _config.Topics!));
			_consumer.Subscribe(_config.Topics);
			_isInitialized = true;

			return Task.CompletedTask;
		});
	}

	private async Task ConsumeNextMessage(IConsumer<string, INotification> consumer,
		CancellationToken cancellationToken)
	{
		const int retryCount = 3;
		var retryPolicy = Policy
			.Handle<KafkaException>()
			.WaitAndRetryAsync(retryCount, attempt => TimeSpan.FromSeconds(5),
				(exception, timeSpan, attempt, context) =>
				{
					_logger.LogWarning("Retry {RetryCount} after Kafka error: {Message}", attempt, exception.Message);
				});

		var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromMinutes(5));
		var policyWrap = Policy.WrapAsync(timeoutPolicy, retryPolicy);

		try
		{
			await policyWrap.ExecuteAsync(async ct =>
			{
				var stopwatch = Stopwatch.StartNew();

				var result = consumer.Consume(TimeSpan.FromSeconds(1));
				if (result == null)
				{
					_logger.LogDebug("No new message received from Kafka.");
					return;
				}

				if (result.Message?.Value == null)
				{
					_logger.LogWarning("Received a message with null value.");
					return;
				}

				_logger.LogInformation("Dispatching event from topic: {Topic}", result.Topic);
				await _eventPublisher.PublishEventAsync(result.Message.Value, ct);

				consumer.Commit(result);
				stopwatch.Stop();

				_logger.LogInformation("Message processed in {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
			}, cancellationToken);
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			_logger.LogDebug("Message consumption canceled.");
		}
		catch (ObjectDisposedException)
		{
			_logger.LogWarning("Attempted to consume from a disposed Kafka consumer.");
			_isInitialized = false;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unhandled error while consuming Kafka message.");
			if (ex is KafkaException)
			{
				_isInitialized = false;
			}
		}
	}

	private async Task EnsureTopicsExistAsync()
	{
		if(_config is null)
			throw new InvalidOperationException(nameof(Config));

		IEnumerable<string> topicNames = _config.Topics;
		string bootstrapServers = _config.ConnectionString;

		var config = new AdminClientConfig { BootstrapServers = bootstrapServers };

		using var adminClient = new AdminClientBuilder(config).Build();

		var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
		var existingTopics = metadata.Topics.Select(t => t.Topic).ToHashSet();

		var missingTopics = topicNames.Where(t => !existingTopics.Contains(t)).ToList();
		if (!missingTopics.Any())
			return;

		_logger.LogWarning("Creating missing Kafka topics: {Missing}", string.Join(", ", missingTopics));

		var topicSpecs = missingTopics.Select(name =>
			new TopicSpecification { Name = name, NumPartitions = 1, ReplicationFactor = 1 });

		await adminClient.CreateTopicsAsync(topicSpecs);
		_logger.LogInformation("Successfully created missing Kafka topics.");
	}
}