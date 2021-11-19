using System.Linq;
using EcommerceDDD.Domain.Quotes;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Infrastructure.Database.Context;

namespace EcommerceDDD.Infrastructure.Domain.Quotes;

public class Quotes : IQuotes
{
    private readonly EcommerceDDDContext _context;

    public Quotes(EcommerceDDDContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task Add(Quote quote, CancellationToken cancellationToken = default)
    {
        await _context.Quotes.AddAsync(quote, cancellationToken);
    }

    public async Task<Quote> GetById(QuoteId quoteId, CancellationToken cancellationToken = default)
    {
        return await _context.Quotes.FirstOrDefaultAsync(x => x.Id == quoteId, cancellationToken);
    }

    public async Task<Quote> GetCurrentQuote(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Quotes.OrderByDescending(t => t.CreationDate)
            .FirstOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);
    }
}