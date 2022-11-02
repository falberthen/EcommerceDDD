using MediatR;

namespace EcommerceDDD.Core.EventBus;

public interface IEventDispatcher
{
    Task DispatchAsync(INotification @event);
}