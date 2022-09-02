using Marten;
using EcommerceDDD.Core.CQRS.QueryHandling;

namespace EcommerceDDD.Customers.Application.GettingCustomerEventHistory;

public class GetCustomerEventHistoryHandler : QueryHandler<GetCustomerEventHistory, IList<CustomerEventHistory>> 
{
    private readonly IQuerySession _querySession;

    public GetCustomerEventHistoryHandler(
        IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public override async Task<IList<CustomerEventHistory>> Handle(GetCustomerEventHistory query, CancellationToken cancellationToken)
    {
        var customerHistory = _querySession.Query<CustomerEventHistory>()
           .Where(c => c.AggregateId == query!.CustomerId.Value);

        if (customerHistory == null)
            throw new ApplicationException($"History for customer {query.CustomerId.Value} was not found.");
        return customerHistory.ToList();
    }
}
