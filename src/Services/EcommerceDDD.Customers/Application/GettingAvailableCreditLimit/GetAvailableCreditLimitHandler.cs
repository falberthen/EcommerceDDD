using Marten;
using EcommerceDDD.Core.CQRS.QueryHandling;
using EcommerceDDD.Customers.Application.GettingCustomerDetails;
using EcommerceDDD.Customers.Api.Application.GettingAvailableCreditLimit;
using EcommerceDDD.Customers.Application.GettingAvailableCreditLimit;

namespace EcommerceDDD.Customers.Api.Application.GettingCustomerDetails;

public class GetAvailableCreditLimitHandler : QueryHandler<GetAvailableCreditLimit, AvailableCreditLimitModel>
{
    private readonly IQuerySession _querySession;

    public GetAvailableCreditLimitHandler(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public override Task<AvailableCreditLimitModel> Handle(GetAvailableCreditLimit query, CancellationToken cancellationToken)
    {        
        var customer = _querySession.Query<CustomerDetails>()
            .FirstOrDefault(c => c.Id == query.CustomerId);

        if (customer == null)
            throw new ApplicationException($"Customer not found");
        
        return Task.FromResult(new AvailableCreditLimitModel(query.CustomerId, customer.AvailableCreditLimit));
    }
}
