namespace EcommerceDDD.Core.Infrastructure.Outbox.Services;

public class OutboxMessageService : IOutboxMessageService
{
    private readonly IDocumentSession _documentSession;
    private readonly ILogger<OutboxMessageService> _logger;

    public OutboxMessageService(
        IDocumentSession documentSession,
        ILogger<OutboxMessageService> logger)
    {
        _documentSession = documentSession;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SaveAsOutboxMessageAsync(IntegrationEvent @event, bool saveChanges = false)
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        _logger.LogInformation("Adding event {@event} to outboxmessages...", @event);
        _documentSession.Store(@event);        

        if (saveChanges)
            await _documentSession.SaveChangesAsync();
    }
}
