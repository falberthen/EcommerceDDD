using Marten;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace EcommerceDDD.Core.Infrastructure.Outbox.Persistence;

public class OutboxMessageRepository : IOutboxMessageRepository
{
    private readonly OutboxDbContext _context;

    public OutboxMessageRepository(OutboxDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await _context.OutboxMessages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<OutboxMessage>> FetchUnprocessedMessagesAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        var messages = await _context.OutboxMessages
            .Where(m => null == m.ProcessedAt)
               .OrderBy(m => m.CreatedAt)
               .Take(batchSize)
               .ToArrayAsync(cancellationToken);

        return messages.ToImmutableList();
    }

    public void Update(OutboxMessage message)
    {
        _context.Update(message);
        _context.SaveChanges();
    }
}
