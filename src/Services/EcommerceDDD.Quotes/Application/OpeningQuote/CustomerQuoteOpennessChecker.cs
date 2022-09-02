using Marten;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Quotes.Application.GettingOpenQuote;

namespace EcommerceDDD.Quotes.Application.OpeningQuote;

/// <summary>
/// Domain service for checking if customer can open a new quote
/// </summary>
public class CustomerQuoteOpennessChecker : ICustomerQuoteOpennessChecker
{
    private readonly IQuerySession _querySession;

    public CustomerQuoteOpennessChecker(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public async Task<bool> CanCustomerOpenNewQuote(CustomerId customerId)
    {
        var quote = await _querySession.Query<QuoteDetails>()
            .FirstOrDefaultAsync(c => c.CustomerId == customerId.Value
            && c.QuoteStatus == QuoteStatus.Open);

        return quote == null || quote.QuoteStatus == QuoteStatus.Open;
    }
}