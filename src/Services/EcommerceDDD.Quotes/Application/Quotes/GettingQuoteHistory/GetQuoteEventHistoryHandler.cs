namespace EcommerceDDD.Quotes.Application.Quotes.GettingQuoteHistory;

public class GetQuoteEventHistoryHandler : IQueryHandler<GetQuoteEventHistory, IList<QuoteEventHistory>> 
{
    private readonly IQuerySession _querySession;

    public GetQuoteEventHistoryHandler(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public Task<IList<QuoteEventHistory>> Handle(GetQuoteEventHistory query, CancellationToken cancellationToken)
    {
        var quoteHistory = _querySession.Query<QuoteEventHistory>()
           .Where(c => c.AggregateId == query.QuoteId.Value);

        return Task.FromResult<IList<QuoteEventHistory>>(quoteHistory.ToList());
    }
}
