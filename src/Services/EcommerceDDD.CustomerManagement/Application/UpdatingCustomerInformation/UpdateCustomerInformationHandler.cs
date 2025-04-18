namespace EcommerceDDD.CustomerManagement.Application.UpdatingCustomerInformation;

public class UpdateCustomerInformationHandler(
	IUserInfoRequester userInfoRequester,
	IQuerySession querySession,
	IEventStoreRepository<Customer> customerWriteRepository
) : ICommandHandler<UpdateCustomerInformation>
{
    private readonly IEventStoreRepository<Customer> _customerWriteRepository = customerWriteRepository
		?? throw new ArgumentNullException(nameof(customerWriteRepository));
	private IUserInfoRequester _userInfoRequester { get; set; } = userInfoRequester
		?? throw new ArgumentNullException(nameof(userInfoRequester));
	private readonly IQuerySession _querySession = querySession
		?? throw new ArgumentNullException(nameof(querySession));

	public async Task HandleAsync(UpdateCustomerInformation command, CancellationToken cancellationToken)
    {
		UserInfo? response = await _userInfoRequester
			.RequestUserInfoAsync();

		var customerDetails = _querySession.Query<CustomerDetails>()
			.FirstOrDefault(c => c.Id == response!.CustomerId)
			?? throw new RecordNotFoundException($"Customer not found.");

		var customer = await _customerWriteRepository
            .FetchStreamAsync(customerDetails.Id)
            ?? throw new ArgumentNullException($"Customer {customerDetails.Id} not found.");

        var customerData = new CustomerData(
            customer.Email,
            command.Name,
            command.ShippingAddress,
            command.CreditLimit);

        customer.UpdateInformation(customerData);

        await _customerWriteRepository
            .AppendEventsAsync(customer, cancellationToken);
    }
}