namespace EcommerceDDD.Core.Infrastructure.Outbox.Persistence;

public interface IOutboxMessageRepository
{
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<OutboxMessage>> FetchUnprocessedMessagesAsync(int batchSize, CancellationToken cancellationToken = default);
    void Update(OutboxMessage message);
}