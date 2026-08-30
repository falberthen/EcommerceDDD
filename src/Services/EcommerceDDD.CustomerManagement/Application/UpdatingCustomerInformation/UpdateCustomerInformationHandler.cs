namespace EcommerceDDD.CustomerManagement.Application.UpdatingCustomerInformation;

public class UpdateCustomerInformationHandler(
	IUserInfoRequester userInfoRequester,
	IEventStoreRepository<Customer> customerWriteRepository
)
{
	private readonly IEventStoreRepository<Customer> _customerWriteRepository = customerWriteRepository
		?? throw new ArgumentNullException(nameof(customerWriteRepository));
	private readonly IUserInfoRequester _userInfoRequester = userInfoRequester
		?? throw new ArgumentNullException(nameof(userInfoRequester));

	public async Task<Result> HandleAsync(UpdateCustomerInformation command, CancellationToken cancellationToken)
    {
		UserInfo? response = _userInfoRequester.GetCurrentUser();

		var customer = await _customerWriteRepository
			.FetchForWritingAsync(response!.CustomerId, cancellationToken: cancellationToken);

		if (customer is null)
			return Result.Fail($"Customer {response!.CustomerId} not found.");

		var customerData = new CustomerData(
            customer.Email,
            command.Name,
            command.ShippingAddress,
            command.CreditLimit);

        customer.UpdateInformation(customerData);

        await _customerWriteRepository
			.AppendEventsAndCommitAsync(customer, cancellationToken);

		return Result.Ok();
    }
}
