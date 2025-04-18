namespace EcommerceDDD.CustomerManagement.Application.GettingCreditLimit;

public class GetCreditLimitHandler(IQuerySession querySession) : IQueryHandler<GetCreditLimit, CreditLimitModel>
{
    private readonly IQuerySession _querySession = querySession
		?? throw new ArgumentNullException(nameof(querySession));

    public Task<CreditLimitModel> HandleAsync(GetCreditLimit query, CancellationToken cancellationToken)
    {        
        var customer = _querySession.Query<CustomerDetails>()
        .FirstOrDefault(c => c.Id == query.CustomerId.Value)
            ?? throw new RecordNotFoundException($"Customer {query.CustomerId} not found.");

        return Task.FromResult(new CreditLimitModel(query.CustomerId.Value, customer.CreditLimit));
    }
}
