namespace EcommerceDDD.OrderProcessing.Application.GettingOrderEventHistory;

public class GetOrderEventHistoryHandler : IQueryHandler<GetOrderEventHistory, IList<OrderEventHistory>> 
{
    private readonly IQuerySession _querySession;

    public GetOrderEventHistoryHandler(
        IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public async Task<IList<OrderEventHistory>> Handle(GetOrderEventHistory query, 
        CancellationToken cancellationToken)
    {
        var quoteHistory = await _querySession.Query<OrderEventHistory>()
           .Where(c => c.AggregateId == query.OrderId.Value)
           .OrderBy(c => c.Timestamp)
           .ToListAsync();

        return quoteHistory.ToList();
    }
}
