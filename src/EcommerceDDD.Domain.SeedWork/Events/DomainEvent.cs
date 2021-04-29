using System;

namespace EcommerceDDD.Domain.Core.Events
{
    public abstract class DomainEvent : Message, IDomainEvent
    {
        public DateTime CreatedAt { get; private set; }

        public DomainEvent()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
