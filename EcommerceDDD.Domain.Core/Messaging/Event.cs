using System;

namespace EcommerceDDD.Domain.Core.Messaging
{
    public abstract class Event : Message, IDomainEvent
    {
        public DateTime CreatedAt { get; private set; }

        public Event()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
