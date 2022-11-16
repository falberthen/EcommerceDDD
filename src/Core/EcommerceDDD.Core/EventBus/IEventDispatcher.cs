using MediatR;
using System.Threading;

namespace EcommerceDDD.Core.EventBus;

public interface IEventDispatcher
{
    Task DispatchAsync(INotification @event, CancellationToken cancellationToken = default);
}