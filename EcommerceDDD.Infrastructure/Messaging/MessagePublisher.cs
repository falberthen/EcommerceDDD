using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Core.Base;
using EcommerceDDD.Domain.Core.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EcommerceDDD.Infrastructure.Messaging
{
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
            Type messageType = StoredEventHelper.GetEventType(message.MessageType);
            var domainEvent = JsonConvert.DeserializeObject(message.Payload, messageType);

            if (messageType != null 
                && domainEvent != null)
            {
                await _mediator.Publish(domainEvent);

                _logger.LogInformation($"\n-------- Message {message.Id} processed at {message.ProcessedAt}\n");
            }
        }
    }
}
