namespace EcommerceDDD.QuoteManagement.Application.Quotes.GettingQuoteHistory;

public class GetQuoteEventHistoryHandler(
	IQuerySession querySession
) : IQueryHandler<GetQuoteEventHistory, IReadOnlyList<QuoteEventHistory>> 
{
    private readonly IQuerySession _querySession = querySession;

    public async Task<IReadOnlyList<QuoteEventHistory>> HandleAsync(GetQuoteEventHistory query, CancellationToken cancellationToken)
    {
		var quoteHistory = await _querySession.Query<QuoteEventHistory>()
		   .Where(c => c.AggregateId == query.QuoteId.Value)
		   .OrderBy(c => c.Timestamp)
			.ToListAsync(cancellationToken);

		return quoteHistory;
    }
}