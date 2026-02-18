namespace EcommerceDDD.Core.Infrastructure.Outbox;

public class DebeziumConnectorSetup : IDebeziumConnectorSetup
{
	private readonly DebeziumSettings _settings;
	private readonly ILogger<DebeziumConnectorSetup> _logger;
	private readonly HttpClient _httpClient;

	public DebeziumConnectorSetup(
		IOptions<DebeziumSettings> debeziumSettings,
		ILogger<DebeziumConnectorSetup> logger,
		IHttpClientFactory httpClientFactory)
	{
		_settings = debeziumSettings?.Value ?? throw new ArgumentNullException(nameof(debeziumSettings));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_httpClient = httpClientFactory.CreateClient("DebeziumClient");
	}

	public async Task StartConfiguringAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			// Add initial delay to allow infrastructure services to stabilize
			// This helps especially when running in VS debug mode where services start simultaneously
			await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

			var config = BuildDebeziumConfig();
			var configJson = JsonConvert.SerializeObject(config);

			_logger.LogInformation("Sending Debezium configuration request...");

			var content = new StringContent(configJson, Encoding.UTF8, MediaTypeNames.Application.Json);

			const int retryCount = 10;
			var retryPolicy = Policy
				.Handle<HttpRequestException>()
				.Or<TaskCanceledException>()
				.Or<OperationCanceledException>()
				.OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
				.WaitAndRetryAsync(retryCount, retry => TimeSpan.FromSeconds(Math.Min(retry * 2, 10)), (outcome, delay, retryAttempt, context) =>
				{
					_logger.LogWarning("Retry {RetryCount}/{MaxRetries} configuring Debezium. Delay: {Delay}s Reason: {Reason}",
						retryAttempt,
						retryCount,
						delay.TotalSeconds,
						outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
				});

			var response = await retryPolicy.ExecuteAsync(async () =>
			{
				using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
				cts.CancelAfter(TimeSpan.FromSeconds(30)); // 30 second timeout per attempt
				return await _httpClient.PutAsync($"{_settings.ConnectorUrl}/config", content, cts.Token);
			});

			if (!response.IsSuccessStatusCode)
			{
				var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
				_logger.LogError("Debezium configuration failed. StatusCode: {StatusCode}, Body: {Body}",
					response.StatusCode, errorBody);
				throw new InvalidOperationException("Debezium connector configuration failed.");
			}

			_logger.LogInformation("Debezium connector configured successfully.");
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			_logger.LogWarning("Debezium configuration cancelled due to application shutdown.");
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unexpected error configuring Debezium connector. The application will continue but outbox pattern may not work correctly.");			
		}
	}

	private Dictionary<string, object> BuildDebeziumConfig()
	{
		return new Dictionary<string, object>
		{
			["database.hostname"] = _settings.DatabaseHostname,
			["database.dbname"] = _settings.DatabaseName,
			["database.port"] = _settings.DatabasePort,
			["database.user"] = _settings.DatabaseUser,
			["database.password"] = _settings.DatabasePassword,
			["database.server.name"] = _settings.DatabaseServerName,
			["topic.prefix"] = _settings.TopicPrefix,
			["slot.name"] = _settings.SlotName,
			["schema.include.list"] = _settings.SchemaIncludeList,
			["table.include.list"] = _settings.TableIncludeList,
			["transforms.outbox.route.topic.replacement"] = _settings.TransformsTopicReplacement.ToLowerInvariant(),
			["connector.class"] = "io.debezium.connector.postgresql.PostgresConnector",
			["tasks.max"] = "1",
			["tombstones.on.delete"] = "false",
			["transforms"] = "outbox",
			["transforms.outbox.type"] = "io.debezium.transforms.outbox.EventRouter",
			["transforms.outbox.table.field.event.id"] = "id",
			["transforms.outbox.table.field.event.key"] = "mt_dotnet_type",
			["transforms.outbox.table.field.event.payload"] = "data",
			["transforms.outbox.route.by.id"] = "id",
			["transforms.outbox.route.by.field"] = "mt_dotnet_type",
			["transforms.outbox.table.fields.additional.placement"] = "mt_dotnet_type:header:eventType",
			["transforms.outbox.debezium.expand.json.payload"] = "true",
			["key.converter"] = "org.apache.kafka.connect.storage.StringConverter",
			["value.converter"] = "org.apache.kafka.connect.storage.StringConverter",
			["internal.key.converter"] = "org.apache.kafka.connect.json.JsonConverter",
			["internal.value.converter"] = "org.apache.kafka.connect.json.JsonConverter",
			["key.converter.schemas.enable"] = "false",
			["value.converter.schemas.enable"] = "false",
			["plugin.name"] = "pgoutput",
			["snapshot.mode"] = "never"
		};
	}
}
