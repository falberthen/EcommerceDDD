using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EcommerceDDD.Domain.Core.Events
{
    public interface IStoredEvents
    {
        void UpdateProcessedAt(StoredEvent message);
        Task StoreRange(List<StoredEvent> messages);
        Task<IList<StoredEvent>> GetByAggregateId(Guid aggregateId, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<StoredEvent>> FetchUnprocessed(int batchSize, CancellationToken cancellationToken);
    }
}