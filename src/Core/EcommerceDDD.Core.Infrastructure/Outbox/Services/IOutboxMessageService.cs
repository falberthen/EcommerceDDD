namespace EcommerceDDD.Core.Infrastructure.Outbox.Services;

public interface IOutboxMessageService
{
    public Task SaveAsOutboxMessageAsync(IntegrationEvent @event, bool saveChanges = false);
}
