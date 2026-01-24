namespace EcommerceDDD.CustomerManagement.Application.RegisteringCustomer;

public class RegisterCustomerHandler(
	IdentityServerClient identityServerClient,
	IEmailUniquenessChecker uniquenessChecker,
	IEventStoreRepository<Customer> customerWriteRepository
) : ICommandHandler<RegisterCustomer>
{
	private readonly IdentityServerClient _identityServerClient = identityServerClient
		?? throw new ArgumentNullException(nameof(_identityServerClient));
	private readonly IEmailUniquenessChecker _uniquenessChecker = uniquenessChecker
		?? throw new ArgumentNullException(nameof(uniquenessChecker));
	private readonly IEventStoreRepository<Customer> _customerWriteRepository = customerWriteRepository
		?? throw new ArgumentNullException(nameof(customerWriteRepository));

	public async Task HandleAsync(RegisterCustomer command, CancellationToken cancellationToken)
	{
		if (!_uniquenessChecker.IsUnique(command.Email))
			throw new BusinessRuleException("This e-mail is already in use.");

		var customerData = new CustomerData(
			command.Email,
			command.Name,
			command.ShippingAddress,
			command.CreditLimit);

		var customer = Customer.Create(customerData);

		var response = await CreateUserForCustomerAsync(command, customer.Id, cancellationToken);

		await _customerWriteRepository
			.AppendEventsAsync(customer, cancellationToken);
	}

	private async Task<UserRegisteredResult?> CreateUserForCustomerAsync(RegisterCustomer command,
		CustomerId customerId, CancellationToken cancellationToken)
	{
		try
		{
			var request = new RegisterUserRequest()
			{
				CustomerId = customerId.Value,
				Email = command.Email,
				Password = command.Password,
				PasswordConfirm = command.PasswordConfirm,
			};

			var accountRequestBuilder = _identityServerClient.Api.V2.Accounts;
			var response = await accountRequestBuilder.Register
				.PostAsync(request, cancellationToken: cancellationToken);

			return response;
		}
		catch (Microsoft.Kiota.Abstractions.ApiException ex)
		{
			throw new ApplicationLogicException(
				$"An error occurred while registering the customer.", ex);
		}		
	}
}