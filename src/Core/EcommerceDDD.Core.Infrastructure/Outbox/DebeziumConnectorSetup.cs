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
		var config = BuildDebeziumConfig();
		var configJson = JsonConvert.SerializeObject(config);

		_logger.LogInformation("Sending Debezium configuration request...");

		var content = new StringContent(configJson, Encoding.UTF8, MediaTypeNames.Application.Json);

		const int retryCount = 5;
		var retryPolicy = Policy
			.Handle<HttpRequestException>()
			.OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
			.WaitAndRetryAsync(retryCount, retry => TimeSpan.FromSeconds(2), (outcome, delay, retryCount, context) =>
			{
				_logger.LogWarning("Retry {RetryCount} configuring Debezium. Delay: {Delay} Reason: {Reason}",
					retryCount,
					delay,
					outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
			});

		var response = await retryPolicy.ExecuteAsync(() =>
			_httpClient.PutAsync($"{_settings.ConnectorUrl}/config", content, cancellationToken));

		if (!response.IsSuccessStatusCode)
		{
			var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
			_logger.LogError("Debezium configuration failed. StatusCode: {StatusCode}, Body: {Body}",
				response.StatusCode, errorBody);
			throw new InvalidOperationException("Debezium connector configuration failed.");
		}

		_logger.LogInformation("Debezium connector configured successfully.");
	}

	private JObject BuildDebeziumConfig()
	{
		return new JObject
		{
			{ "database.hostname", _settings.DatabaseHostname },
			{ "database.dbname", _settings.DatabaseName },
			{ "database.port", _settings.DatabasePort },
			{ "database.user", _settings.DatabaseUser },
			{ "database.password", _settings.DatabasePassword },
			{ "database.server.name", _settings.DatabaseServerName },
			{ "topic.prefix", _settings.TopicPrefix },
			{ "slot.name", _settings.SlotName },
			{ "schema.include.list", _settings.SchemaIncludeList },
			{ "table.include.list", _settings.TableIncludeList },
			{ "transforms.outbox.route.topic.replacement", _settings.TransformsTopicReplacement.ToLowerInvariant() },
			{ "connector.class", "io.debezium.connector.postgresql.PostgresConnector" },
			{ "tasks.max", "1" },
			{ "tombstones.on.delete", "false" },
			{ "transforms", "outbox" },
			{ "transforms.outbox.type", "io.debezium.transforms.outbox.EventRouter" },
			{ "transforms.outbox.table.field.event.id", "id" },
			{ "transforms.outbox.table.field.event.key", "mt_dotnet_type" },
			{ "transforms.outbox.table.field.event.payload", "data" },
			{ "transforms.outbox.route.by.id", "Id" },
			{ "transforms.outbox.route.by.field", "mt_dotnet_type" },
			{ "transforms.outbox.table.fields.additional.placement", "mt_dotnet_type:header:eventType" },
			{ "transforms.outbox.debezium.expand.json.payload", "true" },
			{ "key.converter", "org.apache.kafka.connect.storage.StringConverter" },
			{ "value.converter", "org.apache.kafka.connect.storage.StringConverter" },
			{ "internal.key.converter", "org.apache.kafka.connect.json.JsonConverter" },
			{ "internal.value.converter", "org.apache.kafka.connect.json.JsonConverter" },
			{ "key.converter.schemas.enable", "false" },
			{ "value.converter.schemas.enable", "false" },
			{ "plugin.name", "pgoutput" }
		};
	}
}