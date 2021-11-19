using MediatR;
using Newtonsoft.Json;
using EcommerceDDD.Domain.Core.Events;
using Microsoft.Extensions.Logging;

namespace EcommerceDDD.Infrastructure.Events;

public interface IMessagePublisher
{
    Task Publish(StoredEvent message, System.Threading.CancellationToken cancellationToken);
}

public class MessagePublisher : IMessagePublisher
{
    private readonly IMediator _mediator;
    private readonly ILogger<MessagePublisher> _logger;

    public MessagePublisher(IMediator mediator, ILogger<MessagePublisher> logger)
    {            
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Publish(StoredEvent message, System.Threading.CancellationToken cancellationToken)
    {
        try
        {
            Type messageType = StoredEventHelper.GetEventType(message.MessageType);
            var domainEvent = JsonConvert.DeserializeObject(message.Payload, messageType);

            if (messageType != null && domainEvent != null)
                await _mediator.Publish(domainEvent, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError($"\n--- An error has occurred while publishing message {message.Id}: {ex.Message}\n");
        }
    }        
}