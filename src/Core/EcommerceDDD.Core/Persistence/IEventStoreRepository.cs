using System.Threading;
using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Core.Persistence;

public interface IEventStoreRepository<TA>
    where TA : class, IAggregateRoot<StronglyTypedId<Guid>>
{
    Task<long> AppendEventsAsync(TA aggregate, CancellationToken cancellationToken = default);
    Task<TA> FetchStreamAsync(Guid id, int? version = null, CancellationToken cancellationToken = default);
}