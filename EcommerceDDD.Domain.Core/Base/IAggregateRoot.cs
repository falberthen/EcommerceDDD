using EcommerceDDD.Domain.Core.Messaging;
using System.Collections.Generic;

namespace EcommerceDDD.Domain.Core.Base
{
    /// <summary>
    ///  Aggregate root interface
    /// </summary>
    public interface IAggregateRoot
    {
        IReadOnlyCollection<Event> DomainEvents { get; }
        void ClearDomainEvents();
    }
}
