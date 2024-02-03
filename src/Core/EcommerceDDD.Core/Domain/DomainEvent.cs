namespace EcommerceDDD.Core.Domain;

public record class DomainEvent : IDomainEvent
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}
