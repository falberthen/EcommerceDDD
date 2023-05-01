namespace EcommerceDDD.Customers.Application.GettingCustomerEventHistory;

public class GetCustomerEventHistoryHandler : IQueryHandler<GetCustomerEventHistory, List<CustomerEventHistory>> 
{
    private readonly IQuerySession _querySession;

    public GetCustomerEventHistoryHandler(
        IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public Task<List<CustomerEventHistory>> Handle(GetCustomerEventHistory query, CancellationToken cancellationToken)
    {
        var customerHistory = _querySession.Query<CustomerEventHistory>()
           .Where(c => c.AggregateId == query!.CustomerId.Value);

        if (customerHistory is null)
            throw new RecordNotFoundException($"History for customer {query.CustomerId.Value} was not found.");

        return Task.FromResult(customerHistory.ToList());
    }
}
