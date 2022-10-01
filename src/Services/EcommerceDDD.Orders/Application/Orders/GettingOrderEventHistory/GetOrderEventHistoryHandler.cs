using Marten;
using EcommerceDDD.Core.CQRS.QueryHandling;

namespace EcommerceDDD.Orders.Application.GettingOrderEventHistory;

public class GetOrderEventHistoryHandler : IQueryHandler<GetOrderEventHistory, List<OrderEventHistory>> 
{
    private readonly IQuerySession _querySession;

    public GetOrderEventHistoryHandler(
        IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public Task<List<OrderEventHistory>> Handle(GetOrderEventHistory query, CancellationToken cancellationToken)
    {
        var quoteHistory = _querySession.Query<OrderEventHistory>()
           .Where(c => c.AggregateId == query.OrderId.Value);

        return Task.FromResult(quoteHistory.ToList());
    }
}
