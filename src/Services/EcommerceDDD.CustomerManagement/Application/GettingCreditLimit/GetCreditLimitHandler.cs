namespace EcommerceDDD.CustomerManagement.Application.GettingCreditLimit;

public class GetCreditLimitHandler(IQuerySession querySession) : IQueryHandler<GetCreditLimit, CreditLimitModel>
{
    private readonly IQuerySession _querySession = querySession
		?? throw new ArgumentNullException(nameof(querySession));

    public async Task<Result<CreditLimitModel>> HandleAsync(GetCreditLimit query, CancellationToken cancellationToken)
    {
        var customer = await _querySession.Query<CustomerDetails>()
            .FirstOrDefaultAsync(c => c.Id == query.CustomerId.Value, cancellationToken);

		if (customer is null)
			return Result.Fail<CreditLimitModel>(
				new RecordNotFoundError($"Customer {query.CustomerId} not found."));

		return Result.Ok(
			new CreditLimitModel(query.CustomerId.Value, customer.CreditLimit)
		);
    }
}
