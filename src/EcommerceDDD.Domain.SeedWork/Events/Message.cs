namespace EcommerceDDD.Domain.Core.Events;

public abstract record class Message
{
    public string MessageType { get; init; }
    public Guid AggregateId { get; init; }

    protected Message()
    {
        MessageType = GetType().FullName;
    }
}