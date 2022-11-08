namespace EcommerceDDD.Core.Infrastructure.Outbox;

public class OutboxMessage
{
    public OutboxMessage(string data, string aggregateType)
    {
        Id = Guid.NewGuid();
        Payload = data;
        AggregateId = Guid.NewGuid().ToString();
        AggregateType = aggregateType;
        Type = aggregateType;
        Timestamp = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
    }

    public Guid Id { get; }

    public string Payload { get; }

    public string AggregateId { get; }

    public string AggregateType { get; }

    public string Type { get; }

    public DateTime Timestamp { get; }

    public DateTime? ProcessedAt { get; set; }

    private OutboxMessage() { }
}
