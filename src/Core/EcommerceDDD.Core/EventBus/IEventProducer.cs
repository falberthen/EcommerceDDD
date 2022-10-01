using System.Threading;

namespace EcommerceDDD.Core.EventBus;

public interface IEventProducer
{
    Task PublishAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default);
}