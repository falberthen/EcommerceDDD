namespace EcommerceDDD.Core.EventBus;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : INotification
{ }