namespace EcommerceDDD.QuoteManagement.Application.Quotes.GettingQuoteHistory;

public class GetQuoteEventHistoryHandler(IQuerySession querySession) : IQueryHandler<GetQuoteEventHistory, IList<QuoteEventHistory>> 
{
    private readonly IQuerySession _querySession = querySession;

    public Task<IList<QuoteEventHistory>> Handle(GetQuoteEventHistory query, CancellationToken cancellationToken)
    {
        var quoteHistory = _querySession.Query<QuoteEventHistory>()
           .OrderBy(c => c.Timestamp)
           .Where(c => c.AggregateId == query.QuoteId.Value);

        return Task.FromResult<IList<QuoteEventHistory>>(quoteHistory.ToList());
    }
}
