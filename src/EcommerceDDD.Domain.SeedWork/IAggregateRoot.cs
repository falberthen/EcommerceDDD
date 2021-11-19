using System.Collections.Generic;
using EcommerceDDD.Domain.Core.Events;

namespace EcommerceDDD.Domain.SeedWork;

/// <summary>
///  Aggregate root interface
/// </summary>
public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}