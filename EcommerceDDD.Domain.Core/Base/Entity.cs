using System;
using System.Collections.Generic;
using EcommerceDDD.Domain.Core.Messaging;

namespace EcommerceDDD.Domain.Core.Base
{
    public abstract class Entity
    {
        public Guid Id { get; set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        public IReadOnlyCollection<Event> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent<TE>(TE @event) where TE : Event
        {
            _domainEvents = _domainEvents ?? new List<Event>();
            _domainEvents.Add(@event);
        }
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        private List<Event> _domainEvents;
    }
}
