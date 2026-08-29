namespace EcommerceDDD.CustomerManagement.Application.GettingCustomerDetails;

public class GetCustomerDetailsHandler(
	IUserInfoRequester userInfoRequester,
	IQuerySessionWrapper querySession
)
{
	private readonly IQuerySessionWrapper _querySession = querySession
		?? throw new ArgumentNullException(nameof(querySession));
	private IUserInfoRequester _userInfoRequester { get; set; } = userInfoRequester
		?? throw new ArgumentNullException(nameof(userInfoRequester));

	public async Task<Result<CustomerDetails>> HandleAsync(GetCustomerDetails query,
		CancellationToken cancellationToken)
	{
		UserInfo? userInfo = _userInfoRequester.GetCurrentUser();

		var customer = await _querySession.QueryFirstOrDefaultAsync<CustomerDetails>(
			c => c.Id == userInfo!.CustomerId, cancellationToken);

		if (customer is null)
			return Result.Fail<CustomerDetails>(
				new RecordNotFoundError($"Customer {userInfo!.CustomerId} not found."));

		var details = new CustomerDetails();
		details.Id = customer.Id;
		details.Email = customer.Email;
		details.Name = customer.Name;
		details.ShippingAddress = customer.ShippingAddress;
		details.CreditLimit = customer.CreditLimit;

		return Result.Ok(details);
	}
}
