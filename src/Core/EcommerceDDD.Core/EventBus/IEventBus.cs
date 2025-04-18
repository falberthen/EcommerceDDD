namespace EcommerceDDD.Core.EventBus;

public interface IEventBus
{
	Task PublishEventAsync(INotification @event, CancellationToken cancellationToken);	
}