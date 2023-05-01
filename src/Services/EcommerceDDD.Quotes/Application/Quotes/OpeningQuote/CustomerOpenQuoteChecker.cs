namespace EcommerceDDD.Quotes.Application.Quotes.OpeningQuote;

public class CustomerOpenQuoteChecker : ICustomerOpenQuoteChecker
{
    private readonly IQuerySession _querySession;

    public CustomerOpenQuoteChecker(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public Task<bool> CustomerHasOpenQuote(CustomerId customerId)
    {
        var quote = _querySession.Query<QuoteDetails>()
            .FirstOrDefault(c => c.CustomerId == customerId.Value
            && c.QuoteStatus == QuoteStatus.Open);

        return Task.FromResult(quote is not null);
    }
}