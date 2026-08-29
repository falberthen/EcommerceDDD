namespace EcommerceDDD.CustomerManagement.Application.UpdatingCustomerInformation;

public class UpdateCustomerInformationHandler(
	IUserInfoRequester userInfoRequester,
	IQuerySessionWrapper querySession,
	IEventStoreRepository<Customer> customerWriteRepository
)
{
	private readonly IEventStoreRepository<Customer> _customerWriteRepository = customerWriteRepository
		?? throw new ArgumentNullException(nameof(customerWriteRepository));
	private readonly IUserInfoRequester _userInfoRequester = userInfoRequester
		?? throw new ArgumentNullException(nameof(userInfoRequester));
	private readonly IQuerySessionWrapper _querySession = querySession
		?? throw new ArgumentNullException(nameof(querySession));

	public async Task<Result> HandleAsync(UpdateCustomerInformation command, CancellationToken cancellationToken)
    {
		UserInfo? response = _userInfoRequester.GetCurrentUser();

		var customerDetails = await _querySession.QueryFirstOrDefaultAsync<CustomerDetails>(
			c => c.Id == response!.CustomerId, cancellationToken);

		if (customerDetails is null)
			return Result.Fail("Customer not found.");

		var customer = await _customerWriteRepository
			.FetchStreamAsync(customerDetails.Id, cancellationToken: cancellationToken);

		if (customer is null)
			return Result.Fail($"Customer {customerDetails.Id} not found.");

        var customerData = new CustomerData(
            customer.Email,
            command.Name,
            command.ShippingAddress,
            command.CreditLimit);

        customer.UpdateInformation(customerData);

        await _customerWriteRepository
			.AppendEventsAsync(customer, cancellationToken);

		return Result.Ok();
    }
}
