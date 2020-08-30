using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Core.Messaging;
using EcommerceDDD.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDDD.Infrastructure.Messaging
{
    public class StoredEventRepository : IStoredEventRepository
    {
        private readonly EcommerceDDDContext _dbContext;

        public StoredEventRepository(EcommerceDDDContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public void UpdateProcessedAt(StoredEvent message)
        {
            _dbContext.StoredEvents.Update(message);
        }

        public async Task StoreRange(List<StoredEvent> messages)
        {
            await _dbContext.StoredEvents.AddRangeAsync(messages);
        }

        public async Task<IList<StoredEvent>> GetByAggregateId(Guid aggregateId, CancellationToken cancellationToken)
        {
            var results = await _dbContext.StoredEvents
                .Where(c=>c.AggregateId == aggregateId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<IReadOnlyCollection<StoredEvent>> FetchUnprocessed(int batchSize, CancellationToken cancellationToken)
        {
            var results = await _dbContext.StoredEvents.Where(m => null == m.ProcessedAt)
                .OrderBy(m => m.CreatedAt)
                .Take(batchSize)
                .ToArrayAsync(cancellationToken);

            return results.ToImmutableArray();
        }
    }
}
