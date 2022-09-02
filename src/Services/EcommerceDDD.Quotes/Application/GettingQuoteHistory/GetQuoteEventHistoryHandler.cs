using Marten;
using EcommerceDDD.Core.CQRS.QueryHandling;
using EcommerceDDD.Quotes.Application.GettingQuoteHistory;

namespace EcommerceDDD.Quotes.Api.Application.GettingQuoteHistory;

public class GetQuoteEventHistoryHandler : QueryHandler<GetQuoteEventHistory, IList<QuoteEventHistory>> 
{
    private readonly IQuerySession _querySession;

    public GetQuoteEventHistoryHandler(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public override async Task<IList<QuoteEventHistory>> Handle(GetQuoteEventHistory query, CancellationToken cancellationToken)
    {
        var quoteHistory = _querySession.Query<QuoteEventHistory>()
           .Where(c => c.AggregateId == query!.QuoteId);

        return quoteHistory.ToList();
    }
}
