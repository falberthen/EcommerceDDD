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

	public async Task<Result> HandleAsync(RegisterCustomer command, CancellationToken cancellationToken)
	{
		var isUnique = await _uniquenessChecker.IsUniqueAsync(command.Email, cancellationToken);
		if (!isUnique)
			return Result.Fail("This e-mail is already in use.");

		var customerData = new CustomerData(
			command.Email,
			command.Name,
			command.ShippingAddress,
			command.CreditLimit);

		var customer = Customer.Create(customerData);

		var registerResult = await CreateUserForCustomerAsync(command, customer.Id, cancellationToken);
		if (registerResult.IsFailed)
			return registerResult;

		await _customerWriteRepository
			.AppendEventsAsync(customer, cancellationToken);

		return Result.Ok();
	}

	private async Task<Result> CreateUserForCustomerAsync(RegisterCustomer command,
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
			await accountRequestBuilder.Register
				.PostAsync(request, cancellationToken: cancellationToken);

			return Result.Ok();
		}
		catch (Microsoft.Kiota.Abstractions.ApiException)
		{
			return Result.Fail("An error occurred while registering the customer.");
		}
	}
}
