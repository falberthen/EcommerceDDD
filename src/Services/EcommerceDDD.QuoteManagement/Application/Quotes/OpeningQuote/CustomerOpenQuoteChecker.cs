namespace EcommerceDDD.QuoteManagement.Application.Quotes.OpeningQuote;

public class CustomerOpenQuoteChecker(IQuerySession querySession) : ICustomerOpenQuoteChecker
{
	private readonly IQuerySession _querySession = querySession;

	public async Task<QuoteDetails?> CheckCustomerOpenQuoteAsync(CustomerId customerId, CancellationToken cancellationToken)
	{
		var quote = await _querySession.Query<QuoteDetails>()
			.SingleOrDefaultAsync(c => c.CustomerId == customerId.Value
				&& c.QuoteStatus == QuoteStatus.Open, token: cancellationToken);

		return quote;
	}
}