namespace EcommerceDDD.Core.Domain;

public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>
	where TKey : StronglyTypedId<Guid>
{
	[Identity]
	public Guid AggregateId
	{
		get => Id.Value;
		set { }
	}

	public long Version { get; protected set; }

	public IEnumerable<IDomainEvent> GetUncommittedEvents()
		=> _uncommittedEvents ?? Enumerable.Empty<IDomainEvent>();

	public void ClearUncommittedEvents()
		=> _uncommittedEvents?.Clear();

	protected void AppendEvent(IDomainEvent @event)
		=> (_uncommittedEvents ??= new Queue<IDomainEvent>()).Enqueue(@event);

	// Marten rehydrates an aggregate without running field initializers, so this can still be
	// null on an instance that came back from the event store. Created on first use.
	[JsonIgnore]
	private Queue<IDomainEvent>? _uncommittedEvents = new();
}

//https://event-driven.io/en/using_strongly_typed_ids_with_marten/
