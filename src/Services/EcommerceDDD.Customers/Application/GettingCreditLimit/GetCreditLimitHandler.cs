namespace EcommerceDDD.Customers.Application.GettingCreditLimit;

public class GetCreditLimitHandler : IQueryHandler<GetCreditLimit, CreditLimitModel>
{
    private readonly IQuerySession _querySession;

    public GetCreditLimitHandler(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public Task<CreditLimitModel> Handle(GetCreditLimit query, CancellationToken cancellationToken)
    {        
        var customer = _querySession.Query<CustomerDetails>()
        .FirstOrDefault(c => c.Id == query.CustomerId.Value)
            ?? throw new RecordNotFoundException($"Customer {query.CustomerId} not found.");

        return Task.FromResult(new CreditLimitModel(query.CustomerId.Value, customer.CreditLimit));
    }
}
