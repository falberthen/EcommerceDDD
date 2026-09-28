namespace EcommerceDDD.Core.Persistence;

public interface IEventStoreRepository<TA>
	where TA : class, IAggregateRoot<StronglyTypedId<Guid>>
{
	/// <summary>
	/// Appends the aggregate's uncommitted events and commits them.
	/// Messages are staged in Wolverine's durable outbox on the same session, so the events and the outgoing messages share one transaction. 
	/// Either both land or neither does. Nothing is dispatched until this commit succeeds.
	/// </summary>
	Task<long> AppendEventsAndCommitAsync(TA aggregate, CancellationToken cancellationToken = default,
		params INotification[] messages);

	Task<TA?> FetchForWritingAsync(Guid id, int? version = null, CancellationToken cancellationToken = default);
}
