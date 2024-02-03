namespace EcommerceDDD.CustomerManagement.Application.GettingCustomerEventHistory;

public class GetCustomerEventHistoryHandler : IQueryHandler<GetCustomerEventHistory, IList<CustomerEventHistory>> 
{
    private readonly IQuerySession _querySession;

    public GetCustomerEventHistoryHandler(
        IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public async Task<IList<CustomerEventHistory>> Handle(GetCustomerEventHistory query, 
        CancellationToken cancellationToken)
    {
        var customerHistory = await _querySession.Query<CustomerEventHistory>()
           .Where(c => c.AggregateId == query!.CustomerId.Value)
           .OrderBy(c => c.Timestamp)
           .ToListAsync(cancellationToken);

        return customerHistory.ToList();
    }
}
