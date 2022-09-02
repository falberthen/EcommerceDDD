﻿using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Persistence;
using System.Collections.Generic;
using System.Threading;

namespace EcommerceDDD.Core.Testing;

public class DummyEventStoreRepository<TA> : IEventStoreRepository<TA>
    where TA : class, IAggregateRoot<StronglyTypedId<Guid>>
{
    public List<StreamAction> AggregateStream = new();

    public async Task<long> AppendEventsAsync(TA aggregate, CancellationToken cancellationToken = default)
    {
        var nextVersion = aggregate.Version + 1;
        AggregateStream.Add(new StreamAction(
            aggregate.Id.Value, 
            aggregate, nextVersion, 
            aggregate.GetUncommittedEvents())
        );

        return await Task.FromResult(nextVersion);
    }

    public Task<TA> FetchStream(Guid id, int? version = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(AggregateStream.FirstOrDefault(c=>c.Stream == id)?.Aggregate);

    public record class StreamAction(Guid Stream, TA Aggregate, long ExpectedVersion, IEnumerable<object> Events);
}

