using System;
using EcommerceDDD.Domain.Core.Messaging;

namespace EcommerceDDD.Infrastructure.Messaging
{
    public static class StoredEventHelper
    {
        public static StoredEvent BuildFromDomainEvent<TE>(TE @event, IEventSerializer serializer) where TE : Event
        {
            if (null == @event)
                throw new ArgumentNullException(nameof(@event));
            if (null == serializer)
                throw new ArgumentNullException(nameof(serializer));

            var type = @event.GetType().FullName;
            return new StoredEvent(@event, serializer.Serialize(@event));
        }
    }
}
