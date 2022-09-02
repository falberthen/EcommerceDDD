using Marten;
using EcommerceDDD.Core.CQRS.QueryHandling;

namespace EcommerceDDD.Orders.Application.GettingOrderEventHistory;

public class GetOrderEventHistoryHandler : QueryHandler<GetOrderEventHistory, IList<OrderEventHistory>> 
{
    private readonly IQuerySession _querySession;

    public GetOrderEventHistoryHandler(
        IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public override async Task<IList<OrderEventHistory>> Handle(GetOrderEventHistory query, CancellationToken cancellationToken)
    {
        var quoteHistory = _querySession.Query<OrderEventHistory>()
           .Where(c => c.AggregateId == query!.OrderId);

        return quoteHistory.ToList();
    }
}
