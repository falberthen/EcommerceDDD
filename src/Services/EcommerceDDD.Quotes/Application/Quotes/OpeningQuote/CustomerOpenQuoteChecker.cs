using Marten;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Quotes.Infrastructure.Projections;

namespace EcommerceDDD.Quotes.Application.Quotes.OpeningQuote;

public class CustomerOpenQuoteChecker : ICustomerOpenQuoteChecker
{
    private readonly IQuerySession _querySession;

    public CustomerOpenQuoteChecker(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public async Task<bool> CustomerHasOpenQuote(CustomerId customerId)
    {
        var quote = await _querySession.Query<QuoteDetails>()
            .FirstOrDefaultAsync(c => c.CustomerId == customerId.Value
            && c.QuoteStatus == QuoteStatus.Open);

        return quote is not null;
    }
}