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

	public async Task<CustomerDetails> HandleAsync(GetCustomerDetails query,
		CancellationToken cancellationToken)
	{
		UserInfo? userInfo = await _userInfoRequester
			.RequestUserInfoAsync();
		
		var customer = _querySession.Query<CustomerDetails>()
			.FirstOrDefault(c => c.Id == userInfo!.CustomerId)
			?? throw new RecordNotFoundException($"Customer not found.");

		var details = new CustomerDetails();
		details.Id = customer.Id;
		details.Email = customer.Email;
		details.Name = customer.Name;
		details.ShippingAddress = customer.ShippingAddress;
		details.CreditLimit = customer.CreditLimit;

		return details;
	}
}
