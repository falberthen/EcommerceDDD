namespace EcommerceDDD.Core.Infrastructure.Outbox;

public record class OutboxMessage
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string Type { get; private set; }
    public string Data { get; private set; }
    public DateTime? ProcessedAt { get; set; }

    public OutboxMessage(DateTime createdAt, string type, string data)
    {
        Id = Guid.NewGuid();
        CreatedAt = createdAt;
        Type = type;
        Data = data;
    }

    private OutboxMessage() {}
}