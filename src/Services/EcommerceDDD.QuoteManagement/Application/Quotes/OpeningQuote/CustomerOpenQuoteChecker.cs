namespace EcommerceDDD.QuoteManagement.Application.Quotes.OpeningQuote;

public class CustomerOpenQuoteChecker(IQuerySession querySession) : ICustomerOpenQuoteChecker
{
    private readonly IQuerySession _querySession = querySession;

    public QuoteDetails? CheckCustomerOpenQuote(CustomerId customerId)
    {
        var quote = _querySession.Query<QuoteDetails>()
			.FirstOrDefault(c => c.CustomerId == customerId.Value 
				&& c.QuoteStatus == QuoteStatus.Open);

        return quote;
    }
}