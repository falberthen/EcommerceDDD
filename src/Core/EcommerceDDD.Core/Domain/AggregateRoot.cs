global using System;
global using System.Linq;
global using System.Threading.Tasks;
using Marten.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EcommerceDDD.Core.Domain;

public abstract class AggregateRoot<TKey> : IAggregateRoot<TKey>
    where TKey : StronglyTypedId<Guid>
{
    public TKey Id { get; set; } = default!;

    [Identity]
    public Guid AggregateId
    {
        get => Id.Value;
        set {}
    }

    public long Version { get; protected set; }

    public IEnumerable<IDomainEvent> GetUncommittedEvents()
    {
        return _uncommittedEvents;
    }

    public void ClearUncommittedEvents()
    {
        _uncommittedEvents.Clear();
    }

    protected void AppendEvent(IDomainEvent @event)
    {
        _uncommittedEvents.Enqueue(@event);
    }

    [JsonIgnore]
    private readonly Queue<IDomainEvent> _uncommittedEvents = new Queue<IDomainEvent>();
}

//https://event-driven.io/en/using_strongly_typed_ids_with_marten/