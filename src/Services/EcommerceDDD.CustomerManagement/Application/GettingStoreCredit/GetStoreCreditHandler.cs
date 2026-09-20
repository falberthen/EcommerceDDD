namespace EcommerceDDD.CustomerManagement.Application.GettingStoreCredit;

public class GetStoreCreditHandler(IQuerySession querySession)
{
    private readonly IQuerySession _querySession = querySession
		?? throw new ArgumentNullException(nameof(querySession));

    public async Task<Result<StoreCreditModel>> HandleAsync(GetStoreCredit query, CancellationToken cancellationToken)
    {
        var customer = await _querySession.Query<CustomerDetails>()
            .FirstOrDefaultAsync(c => c.Id == query.CustomerId.Value, cancellationToken);

		if (customer is null)
			return Result.Fail<StoreCreditModel>(
				new RecordNotFoundError($"Customer {query.CustomerId} not found."));

		return Result.Ok(
			new StoreCreditModel(query.CustomerId.Value, customer.StoreCredit)
		);
    }
}
