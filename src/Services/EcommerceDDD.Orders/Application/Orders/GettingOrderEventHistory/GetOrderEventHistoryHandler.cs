namespace EcommerceDDD.Orders.Application.GettingOrderEventHistory;

public class GetOrderEventHistoryHandler : IQueryHandler<GetOrderEventHistory, IList<OrderEventHistory>> 
{
    private readonly IQuerySession _querySession;

    public GetOrderEventHistoryHandler(
        IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public Task<IList<OrderEventHistory>> Handle(GetOrderEventHistory query, CancellationToken cancellationToken)
    {
        var quoteHistory = _querySession.Query<OrderEventHistory>()
           .Where(c => c.AggregateId == query.OrderId.Value);

        return Task.FromResult<IList<OrderEventHistory>>(quoteHistory.ToList());
    }
}
