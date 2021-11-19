using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.SeedWork;

namespace EcommerceDDD.Domain.Quotes;

public interface IQuotes : IRepository<Quote>
{
    Task Add(Quote quote, CancellationToken cancellationToken = default);
    Task<Quote> GetById(QuoteId quoteId, CancellationToken cancellationToken = default);
    Task<Quote> GetCurrentQuote(CustomerId customerId, CancellationToken cancellationToken = default);
}