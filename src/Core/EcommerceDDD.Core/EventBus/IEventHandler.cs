namespace EcommerceDDD.Core.EventBus;

public interface IEventHandler<TEvent> where TEvent : INotification
{
	Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}