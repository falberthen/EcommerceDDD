using Newtonsoft.Json;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Core.Reflection;
using Microsoft.Extensions.Logging;
using EcommerceDDD.Core.Infrastructure.Outbox.Persistence;

namespace EcommerceDDD.Core.Infrastructure.Outbox.Services;

public class OutboxMessageService : IOutboxMessageService
{
    private readonly IEventProducer _eventProducer;
    private readonly IOutboxMessageRepository _outboxMessageRepository;
    private readonly ILogger<OutboxMessageService> _logger;

    public OutboxMessageService(
        IEventProducer eventProducer,
        IOutboxMessageRepository outboxMessageRepository,
        ILogger<OutboxMessageService> logger)
    {
        _eventProducer = eventProducer ?? throw new ArgumentNullException(nameof(eventProducer));
        _outboxMessageRepository = outboxMessageRepository ?? throw new ArgumentNullException(nameof(outboxMessageRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OutboxMessage> SaveAsOutboxMessageAsync(INotification @event)
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        var eventTypeName = @event.GetType().Name;
        var serializedEvent = JsonConvert.SerializeObject(@event);
        var outboxMessage = new OutboxMessage(serializedEvent, eventTypeName);
        _logger.LogInformation("Adding outbox message {outboxMessage}...", eventTypeName);

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
                .DeserializeObject(message.Payload, eventType, settings) as INotification;

            await _eventProducer.PublishAsync(@event!, cancellationToken);
            message.ProcessedAt = DateTime.UtcNow;

            _outboxMessageRepository.Update(message);
        }        
    }
}
