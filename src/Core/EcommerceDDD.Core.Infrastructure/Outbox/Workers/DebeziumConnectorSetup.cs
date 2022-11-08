using Polly;
using System.Text;
using Newtonsoft.Json;
using System.Net.Mime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        IOptions<OutboxSettings> outboxSettings,
        ILogger<DebeziumConnectorSetup> logger)
    {
        _debeziumSettings = outboxSettings.Value.DebeziumSettings ?? throw new ArgumentNullException(nameof(outboxSettings));
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
                var debeziumConfig = new DebeziumConnectorConfig();

                debeziumConfig.DatabaseHostname = _debeziumSettings.DatabaseHostname;
                debeziumConfig.DatabaseServerName = _debeziumSettings.DatabaseServerName;
                debeziumConfig.DatabasePort = _debeziumSettings.DatabasePort;
                debeziumConfig.DatabaseUser = _debeziumSettings.DatabaseUser;
                debeziumConfig.DatabasePassword = _debeziumSettings.DatabasePassword;
                debeziumConfig.DatabaseName = _debeziumSettings.DatabaseName;
                debeziumConfig.TopicPrefix = _debeziumSettings.TopicPrefix;
                debeziumConfig.TransformsOutboxRouteTopicReplacement = _debeziumSettings.TransformsTopicReplacement;
                debeziumConfig.SlotName = _debeziumSettings.SlotName;

                var config = new StringContent(
                        JsonConvert.SerializeObject(debeziumConfig),
                        Encoding.UTF8,
                        MediaTypeNames.Application.Json);

                _logger.LogInformation("Configuring debezium with config: {config}", config);

                var response = await httpClient.PutAsync(
                    _debeziumSettings.ConnectorUrl,
                    config
                );

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Was not possible to configure Debezium. Status code: {statusCode}", response.StatusCode);
                    throw new Exception("Was not possible to configure Debezium outbox.");
                }
            });
    }
}
