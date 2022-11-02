using Newtonsoft.Json;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Core.Reflection;
using EcommerceDDD.Core.Infrastructure.Outbox.Persistence;

namespace EcommerceDDD.Core.Infrastructure.Outbox.Services;

public class OutboxMessageService : IOutboxMessageService
{
    private readonly IEventProducer _eventProducer;
    private readonly IOutboxMessageRepository _outboxMessageRepository;

    public OutboxMessageService(
        IEventProducer eventProducer,
        IOutboxMessageRepository outboxMessageRepository)
    {
        _eventProducer = eventProducer ?? throw new ArgumentNullException(nameof(eventProducer));
        _outboxMessageRepository = outboxMessageRepository ?? throw new ArgumentNullException(nameof(outboxMessageRepository));
    }

    public async Task<OutboxMessage> SaveAsOutboxMessageAsync(INotification @event)
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        var integrationEventData = JsonConvert.SerializeObject(@event);
        var eventType = @event.GetType();
        var outboxMessage = new OutboxMessage(DateTime.UtcNow, eventType.Name, integrationEventData);

        await _outboxMessageRepository.AddAsync(outboxMessage);
        return outboxMessage;
    }

    public async Task<IEnumerable<OutboxMessage>> FetchUnprocessedMessagesAsync(int batchSize, CancellationToken cancellationToken)
    {
        var messages = await _outboxMessageRepository.FetchUnprocessedMessagesAsync(batchSize, cancellationToken);
        return messages;
    }

    public async Task ProcessMessageAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        var eventType = TypeGetter.GetTypeFromCurrencDomainAssembly(message.Type);

        if (eventType != null)
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new PrivateResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };

            var @event = JsonConvert
                .DeserializeObject(message.Data, eventType, settings) as INotification;

            await _eventProducer.PublishAsync(@event!, cancellationToken);
            message.ProcessedAt = DateTime.UtcNow;

            _outboxMessageRepository.Update(message);
        }        
    }
}
