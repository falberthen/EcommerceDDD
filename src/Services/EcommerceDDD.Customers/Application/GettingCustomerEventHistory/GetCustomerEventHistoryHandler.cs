namespace EcommerceDDD.Customers.Application.GettingCustomerEventHistory;

public class GetCustomerEventHistoryHandler : IQueryHandler<GetCustomerEventHistory, IList<CustomerEventHistory>> 
{
    private readonly IQuerySession _querySession;

    public GetCustomerEventHistoryHandler(
        IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public Task<IList<CustomerEventHistory>> Handle(GetCustomerEventHistory query, CancellationToken cancellationToken)
    {
        var customerHistory = _querySession.Query<CustomerEventHistory>()
           .Where(c => c.AggregateId == query!.CustomerId.Value);

        if (customerHistory is null)
            throw new RecordNotFoundException($"History for customer {query.CustomerId.Value} was not found.");

        return Task.FromResult<IList<CustomerEventHistory>>(customerHistory.ToList());
    }
}
