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
        private readonly ILogger<MessagePublisher> _logger;
        private readonly IMediator _mediator;

        public MessagePublisher(ILogger<MessagePublisher> logger, IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator;
        }

        public async Task Publish(StoredEvent message, System.Threading.CancellationToken cancellationToken)
        {
            Type messageType = GetType(message.MessageType);
            var domainEvent = JsonConvert.DeserializeObject(message.Payload, messageType);

            if (messageType != null 
                && domainEvent != null)
            {
                await _mediator.Publish(domainEvent);
                _logger.LogInformation($"message {message.Id} processed!");
            }
        }

        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
    }
}
