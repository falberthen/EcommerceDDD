namespace EcommerceDDD.Core.Testing;

public class DummyEventStoreRepository<TA> : IEventStoreRepository<TA>
    where TA : class, IAggregateRoot<StronglyTypedId<Guid>>
{
    public List<StreamAction> AggregateStream = new();
    public List<INotification> PublishedMessages = new();

    public async Task<long> AppendEventsAndCommitAsync(TA aggregate, CancellationToken cancellationToken = default,
        params INotification[] messages)
    {
        var nextVersion = aggregate.Version + 1;
        AggregateStream.Add(new StreamAction(
            aggregate.Id.Value, 
            aggregate, nextVersion, 
            aggregate.GetUncommittedEvents())
        );

        PublishedMessages.AddRange(messages);

        return await Task.FromResult(nextVersion);
    }

    public Task<TA> FetchForWritingAsync(Guid id, int? version = null, CancellationToken cancellationToken = default) 
		=> Task.FromResult(AggregateStream.FirstOrDefault(c=>c.Stream == id)?.Aggregate!);

    public record class StreamAction(
		Guid Stream, 
		TA Aggregate, 
		long ExpectedVersion, 
		IEnumerable<object> Events
	);
}
