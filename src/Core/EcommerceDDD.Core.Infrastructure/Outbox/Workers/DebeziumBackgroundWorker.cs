namespace EcommerceDDD.Core.Infrastructure.Outbox.Workers;

public class DebeziumBackgroundWorker : BackgroundWorker
{
    public DebeziumBackgroundWorker(ILogger<DebeziumBackgroundWorker> logger, IDebeziumConnectorSetup connectorSetup)
        : base(logger, connectorSetup.StartConfiguringAsync)
    {
    }
}