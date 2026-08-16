namespace EcommerceDDD.CustomerManagement.Application.GettingCustomerDetails;

public class GetCustomerDetailsHandler(
	IUserInfoRequester userInfoRequester,
	IQuerySession querySession
) : IQueryHandler<GetCustomerDetails, CustomerDetails>
{
	private readonly IQuerySession _querySession = querySession
		?? throw new ArgumentNullException(nameof(querySession));
	private IUserInfoRequester _userInfoRequester { get; set; } = userInfoRequester
		?? throw new ArgumentNullException(nameof(userInfoRequester));

	public Task<Result<CustomerDetails>> HandleAsync(GetCustomerDetails query,
		CancellationToken cancellationToken)
	{
		UserInfo? userInfo = _userInfoRequester.GetCurrentUser();

		var customer = _querySession.Query<CustomerDetails>()
			.FirstOrDefault(c => c.Id == userInfo!.CustomerId);

		if (customer is null)
			return Task.FromResult(Result.Fail<CustomerDetails>(
				new RecordNotFoundError($"Customer {userInfo!.CustomerId} not found.")));

		var details = new CustomerDetails();
		details.Id = customer.Id;
		details.Email = customer.Email;
		details.Name = customer.Name;
		details.ShippingAddress = customer.ShippingAddress;
		details.CreditLimit = customer.CreditLimit;

		return Task.FromResult(Result.Ok(details));
	}
}
