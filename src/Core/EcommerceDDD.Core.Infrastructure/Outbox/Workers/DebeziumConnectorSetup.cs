namespace EcommerceDDD.Core.Infrastructure.Outbox.Workers;

public interface IDebeziumConnectorSetup
{
    Task StartConfiguringAsync(CancellationToken cancellationToken = default);
}

public class DebeziumConnectorSetup : IDebeziumConnectorSetup
{
    private readonly DebeziumSettings _debeziumSettings;
    private readonly ILogger<DebeziumConnectorSetup> _logger;

    public DebeziumConnectorSetup(
        IOptions<DebeziumSettings> debeziumSettings,
        ILogger<DebeziumConnectorSetup> logger)
    {
        _debeziumSettings = debeziumSettings.Value ?? throw new ArgumentNullException(nameof(debeziumSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartConfiguringAsync(CancellationToken cancellationToken = default)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        await retryPolicy.ExecuteAsync(
            async () =>
            {
                using var httpClient = new HttpClient();

                var debeziumConfig = new JObject
                {
                    { "database.hostname", _debeziumSettings.DatabaseHostname },
                    { "database.dbname", _debeziumSettings.DatabaseName },
                    { "database.port", _debeziumSettings.DatabasePort },
                    { "database.user", _debeziumSettings.DatabaseUser },
                    { "database.password", _debeziumSettings.DatabasePassword },
                    { "database.server.name", _debeziumSettings.DatabaseServerName },
                    { "topic.prefix", _debeziumSettings.TopicPrefix },
                    { "slot.name", _debeziumSettings.SlotName },
                    { "schema.include.list", _debeziumSettings.SchemaIncludeList },
                    { "table.include.list", _debeziumSettings.TableIncludeList },
                    { "transforms.outbox.route.topic.replacement", _debeziumSettings.TransformsTopicReplacement },
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

                var debeziumConfigData = JsonConvert.SerializeObject(debeziumConfig);
                var configContent = new StringContent(
                    JsonConvert.SerializeObject(debeziumConfig),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json);

                _logger.LogInformation("Configuring debezium with config: {debeziumConfigData}", debeziumConfigData);
                var response = await httpClient.PutAsync(
                    $"{_debeziumSettings.ConnectorUrl}/config",
                    configContent
                );

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Was not possible to configure Debezium. Status code: {statusCode}", response.StatusCode);
                    throw new Exception("Was not possible to configure Debezium outbox.");
                }
            });
    }
}

// https://debezium.io/documentation/reference/stable/transformations/outbox-event-router.html