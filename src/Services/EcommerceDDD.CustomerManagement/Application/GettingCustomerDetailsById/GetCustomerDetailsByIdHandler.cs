namespace EcommerceDDD.CustomerManagement.Api.Application.GettingCustomerDetailsById;

public class GetCustomerDetailsByIdHandler(
    IQuerySession querySession) : IQueryHandler<GetCustomerDetailsById, CustomerDetails>
{
    private readonly IQuerySession _querySession = querySession
		?? throw new ArgumentNullException(nameof(querySession));

    public async Task<Result<CustomerDetails>> HandleAsync(GetCustomerDetailsById query,
        CancellationToken cancellationToken)
    {
        var customer = await _querySession.Query<CustomerDetails>()
            .FirstOrDefaultAsync(c => c.Id == query.CustomerId.Value);

		if (customer is null)
			return Result.Fail<CustomerDetails>(
				new RecordNotFoundError($"Customer {query.CustomerId} not found."));

		var details = new CustomerDetails()
		{
			Id = customer.Id,
			Email = customer.Email,
			Name = customer.Name,
			ShippingAddress = customer.ShippingAddress,
			CreditLimit = customer.CreditLimit
		};

        return Result.Ok(details);
    }
}
