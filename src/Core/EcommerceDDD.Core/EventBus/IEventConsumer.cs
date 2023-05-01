namespace EcommerceDDD.Core.EventBus;

public interface IEventConsumer
{
    Task StartConsumeAsync(CancellationToken cancellationToken = default);
}
