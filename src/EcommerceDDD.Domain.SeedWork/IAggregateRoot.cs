using EcommerceDDD.Domain.Core.Events;
using System.Collections.Generic;

namespace EcommerceDDD.Domain.SeedWork
{
    /// <summary>
    ///  Aggregate root interface
    /// </summary>
    public interface IAggregateRoot
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}
