using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using EcommerceDDD.Domain.Core.Messaging;

namespace EcommerceDDD.Infrastructure.Messaging
{
    public class EventSerializer : IEventSerializer
    {
        public string Serialize<TE>(TE @event) where TE : Event
        {
            if (null == @event)
                throw new ArgumentNullException(nameof(@event));
            var eventType = @event.GetType();
            var result = JsonSerializer.Serialize(@event, eventType);
            return result;
        }
    }
}
