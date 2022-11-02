using MediatR;
using System.Threading;

namespace EcommerceDDD.Core.EventBus;

public interface IEventProducer
{
    Task PublishAsync(INotification @event, CancellationToken cancellationToken = default);
}