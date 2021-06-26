using System;
using System.Text.Json;
using EcommerceDDD.Domain.Core.Events;

namespace EcommerceDDD.Infrastructure.Events
{
    public interface IEventSerializer
    {
        string Serialize<TE>(TE @event) where TE : DomainEvent;
    }

    public class EventSerializer : IEventSerializer
    {
        public string Serialize<TE>(TE @event) where TE : DomainEvent
        {
            if (null == @event)
                throw new ArgumentNullException(nameof(@event));

            var eventType = @event.GetType();
            var result = JsonSerializer.Serialize(@event, eventType);
            return result;
        }
    }
}
