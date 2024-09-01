namespace EcommerceDDD.CustomerManagement.Application.GettingCustomerEventHistory;

public class GetCustomerEventHistoryHandler(
    IQuerySession querySession) : IQueryHandler<GetCustomerEventHistory, IList<CustomerEventHistory>> 
{
    private readonly IQuerySession _querySession = querySession
		?? throw new ArgumentNullException(nameof(querySession));

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
