namespace EcommerceDDD.OrderProcessing.Application.GettingOrderEventHistory;

public class GetOrderEventHistoryHandler(
    IQuerySession querySession
) : IQueryHandler<GetOrderEventHistory, IList<OrderEventHistory>> 
{
	private readonly IQuerySession _querySession = querySession;

	public async Task<IList<OrderEventHistory>> HandleAsync(GetOrderEventHistory query, 
        CancellationToken cancellationToken)
    {
        var quoteHistory = await _querySession.Query<OrderEventHistory>()
           .Where(c => c.AggregateId == query.OrderId.Value)
           .OrderBy(c => c.Timestamp)
           .ToListAsync(cancellationToken);

        return quoteHistory.ToList();
    }
}
