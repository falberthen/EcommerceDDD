namespace EcommerceDDD.CustomerManagement.Application.RegisteringCustomer;

public class RegisterCustomerHandler(
	IIdentityService identityService,
	IEmailUniquenessChecker uniquenessChecker,
	IEventStoreRepository<Customer> customerWriteRepository
) : ICommandHandler<RegisterCustomer>
{
	private readonly IIdentityService _identityService = identityService
		?? throw new ArgumentNullException(nameof(identityService));
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
			await _identityService.RegisterUserAsync(
				customerId.Value,
				command.Email,
				command.Password,
				command.PasswordConfirm,
				cancellationToken);

			return Result.Ok();
		}
		catch (Exception)
		{
			return Result.Fail("An error occurred while registering the customer.");
		}
	}
}
