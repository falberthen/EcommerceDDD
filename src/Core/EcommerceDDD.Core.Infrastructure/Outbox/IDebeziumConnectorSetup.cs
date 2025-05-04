namespace EcommerceDDD.Core.Infrastructure.Outbox;

public interface IDebeziumConnectorSetup
{
	Task StartConfiguringAsync(CancellationToken cancellationToken = default);
}

