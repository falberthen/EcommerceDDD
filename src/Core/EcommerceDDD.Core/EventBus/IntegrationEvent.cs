namespace EcommerceDDD.Core.EventBus;

public class IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public string EventName { get; } // Event name identifier
    public string JSON_Payload { get; } // Serialized data

    public static IntegrationEvent FromNotification(
        INotification domainEvent)
    {
        return new IntegrationEvent(domainEvent);
    }

    public IntegrationEvent() { }

    private IntegrationEvent(INotification @event)
    {
        EventName = @event.GetType().Name;
        JSON_Payload = JsonConvert.SerializeObject(@event);
    }    
}
