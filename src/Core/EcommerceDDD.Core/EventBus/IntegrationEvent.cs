namespace EcommerceDDD.Core.EventBus;

/// <summary>
/// Marker base for contracts exchanged between bounded contexts.
/// </summary>
public class IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}
