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
	private readonly SemaphoreSlim _initializationLock = new SemaphoreSlim(1, 1);

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
					
					// Add delay to prevent tight loop if initialization keeps failing
					if (!_isInitialized)
					{
						await Task.Delay(5000, cancellationToken);
						continue;
					}
				}

				try
				{
					await ConsumeNextMessage(_consumer!, cancellationToken);
				}
				catch (Exception ex) when (
					ex is KafkaException ||
					ex is ObjectDisposedException ||
					ex is OperationCanceledException)
				{
					if (cancellationToken.IsCancellationRequested)
						break;

					_logger.LogWarning(ex, "Error during message consumption. Will attempt recovery.");
					_isInitialized = false;
					await Task.Delay(1000, cancellationToken);
				}
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
			CloseConsumer();
		}
	}

	private void CloseConsumer()
	{
		try
		{
			if (_consumer != null)
			{
				_logger.LogInformation("Closing Kafka consumer...");
				_consumer.Close();
				_consumer.Dispose();
				_consumer = null;
				_logger.LogInformation("Kafka consumer closed and disposed.");
			}
		}
		catch (ObjectDisposedException)
		{
			_logger.LogWarning("Kafka consumer already disposed before close.");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error during Kafka consumer disposal.");
		}
		finally
		{
			_isInitialized = false;
		}
	}

	private async Task InitializeConsumerWithRetryAsync(CancellationToken cancellationToken)
	{
		if (_isInitialized)
			return;

		// Use a lock to prevent multiple initialization attempts in parallel
		await _initializationLock.WaitAsync(cancellationToken);

		try
		{
			// Double-check after acquiring the lock
			if (_isInitialized)
				return;

			var retryPolicy = Policy
				.Handle<Exception>() // Handle any exception during initialization
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
				// Close and dispose any existing consumer
				CloseConsumer();

				_logger.LogInformation("Creating new Kafka consumer with connection to {BootstrapServers}", _config.ConnectionString);

				var consumerConfig = new ConsumerConfig
				{
					GroupId = _config.Group,
					BootstrapServers = _config.ConnectionString,
					AutoOffsetReset = AutoOffsetReset.Earliest,
					EnableAutoCommit = false,
					AllowAutoCreateTopics = true, // Changed to true for better resilience
					MaxPollIntervalMs = 300000,
					SessionTimeoutMs = 30000, // Increased timeout
					EnablePartitionEof = true,
					// Explicitly configure security to use plaintext
					SecurityProtocol = SecurityProtocol.Plaintext,
					// Disable SSL certificate verification
					SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.None,
					EnableSslCertificateVerification = false,
					// Add reconnect settings
					SocketTimeoutMs = 60000,
					SocketKeepaliveEnable = true,
					// Better error handling
					Debug = "broker,topic,msg"
				};

				_consumer = new ConsumerBuilder<string, INotification>(consumerConfig)
					.SetKeyDeserializer(Deserializers.Utf8)
					.SetValueDeserializer(_serializer)
					.SetErrorHandler((_, error) =>
					{
						_logger.LogError("Kafka error: {Reason} (Code: {Code})", error.Reason, error.Code);
						if (error.IsFatal)
						{
							_logger.LogCritical("Fatal Kafka error occurred. Consumer will need to be reinitialized.");
							_isInitialized = false;
						}
					})
					.SetLogHandler((_, message) =>
						_logger.LogDebug("Kafka: {Message} (Level: {Level})", message.Message, message.Level))
					.Build();

				await EnsureTopicsExistAsync();

				_logger.LogInformation("Subscribing to topics: {Topics}", string.Join(", ", _config.Topics!));
				try
				{
					_consumer.Subscribe(_config.Topics);
					_isInitialized = true;
					_logger.LogInformation("Successfully subscribed to Kafka topics");
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to subscribe to topics");
					throw;
				}
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to initialize Kafka consumer after multiple retries");
			_isInitialized = false;
			throw;
		}
		finally
		{
			_initializationLock.Release();
		}
	}

	private async Task ConsumeNextMessage(IConsumer<string, INotification> consumer,
		CancellationToken cancellationToken)
	{
		try
		{
			// Use a smaller timeout for Consume to be more responsive to cancellation
			var result = consumer.Consume(TimeSpan.FromMilliseconds(500));
			if (result is null)
			{
				// No message available, just return
				return;
			}

			if (result.IsPartitionEOF)
			{
				_logger.LogDebug("Reached end of partition for topic {Topic}, partition {Partition}, offset {Offset}",
					result.Topic, result.Partition, result.Offset);
				return;
			}

			if (result.Message?.Value == null)
			{
				_logger.LogWarning("Received a message with null value from topic {Topic}", result.Topic);
				consumer.Commit(result);
				return;
			}

			var stopwatch = Stopwatch.StartNew();
			_logger.LogInformation("Processing message from topic {Topic} at offset {Offset}", result.Topic, result.Offset);

			// Process the message
			await _eventPublisher
				.PublishEventAsync(result.Message.Value, cancellationToken);

			// Commit offset after successful processing
			consumer.Commit(result);
			stopwatch.Stop();

			_logger.LogInformation("Successfully processed message from {Topic} in {ElapsedMilliseconds}ms",
				result.Topic, stopwatch.ElapsedMilliseconds);
		}
		catch (ConsumeException ex)
		{
			_logger.LogError(ex, "Error consuming message");
			if (ex.Error.IsFatal)
			{
				_logger.LogCritical("Fatal Kafka error. Forcing reinitialization.");
				_isInitialized = false;
			}
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			// Clean cancellation, just return
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unexpected error during message consumption");
			if (ex is KafkaException or ObjectDisposedException)
			{
				_isInitialized = false;
			}
		}
	}

	private async Task EnsureTopicsExistAsync()
	{
		if (_config is null)
			throw new InvalidOperationException(nameof(Config));

		try
		{
			IEnumerable<string> topicNames = _config.Topics;
			string bootstrapServers = _config.ConnectionString;

			var adminConfig = new AdminClientConfig
			{
				BootstrapServers = bootstrapServers,
				SecurityProtocol = SecurityProtocol.Plaintext,
				SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.None,
				EnableSslCertificateVerification = false,
			};

			using var adminClient = new AdminClientBuilder(adminConfig).Build();

			_logger.LogInformation("Retrieving Kafka metadata to check for topics existence");
			var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
			var existingTopics = metadata.Topics.Select(t => t.Topic).ToHashSet();

			var missingTopics = topicNames?.Where(t => !existingTopics.Contains(t)).ToList();
			if (missingTopics is null || !missingTopics.Any())
			{
				_logger.LogInformation("All required Kafka topics already exist");
				return;
			}

			_logger.LogWarning("Creating missing Kafka topics: {Missing}", string.Join(", ", missingTopics));

			var topicSpecs = missingTopics.Select(name =>
				new TopicSpecification { Name = name, NumPartitions = 1, ReplicationFactor = 1 });

			await adminClient.CreateTopicsAsync(topicSpecs);
			_logger.LogInformation("Successfully created missing Kafka topics");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error ensuring Kafka topics exist. Will continue with initialization.");
		}
	}
}