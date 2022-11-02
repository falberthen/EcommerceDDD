namespace EcommerceDDD.Core.Infrastructure.Outbox.Services;

public interface IOutboxMessageService
{
    public Task<OutboxMessage> SaveAsOutboxMessageAsync(INotification @event);
    public Task<IEnumerable<OutboxMessage>> FetchUnprocessedMessagesAsync(int batchSize, CancellationToken cancellationToken);
    public Task ProcessMessageAsync(OutboxMessage message, CancellationToken cancellationToken);
}
