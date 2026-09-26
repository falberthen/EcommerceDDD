namespace EcommerceDDD.CustomerManagement.Application.RegisteringCustomer;

/// <summary>
/// Registers the customer first and its user second, both under the same id.
/// </summary>
public class RegisterCustomerHandler(
	IIdentityService identityService,
	ICustomerEmailLookup customerEmailLookup,
	IEventStoreRepository<Customer> customerWriteRepository
)
{
	private readonly IIdentityService _identityService = identityService
		?? throw new ArgumentNullException(nameof(identityService));
	private readonly ICustomerEmailLookup _customerEmailLookup = customerEmailLookup
		?? throw new ArgumentNullException(nameof(customerEmailLookup));
	private readonly IEventStoreRepository<Customer> _customerWriteRepository = customerWriteRepository
		?? throw new ArgumentNullException(nameof(customerWriteRepository));

	public async Task<Result> HandleAsync(RegisterCustomer command, CancellationToken cancellationToken)
	{
		var customerData = new CustomerData(
			command.Email,
			command.Name,
			command.ShippingAddress,
			command.StoreCredit);

		var existingCustomerId = await _customerEmailLookup
			.FindCustomerIdAsync(command.Email, cancellationToken);

		var customer = existingCustomerId is null
			? Customer.Create(customerData)
			: await _customerWriteRepository.FetchForWritingAsync(existingCustomerId.Value, cancellationToken: cancellationToken)
				?? throw new InvalidOperationException($"Customer {existingCustomerId} is projected but has no stream.");

		if (existingCustomerId is null)
			await _customerWriteRepository.AppendEventsAndCommitAsync(customer, cancellationToken);

		// Transport failures throw on purpose, so the customer stays unfinished and a retry completes it.
		var registration = await _identityService.RegisterUserAsync(
			customer.Id.Value,
			command.Email,
			command.Password,
			command.PasswordConfirm,
			cancellationToken);

		if (registration.Status == UserRegistrationStatus.EmailTaken)
			return Result.Fail(new ValidationError(
				"An account with this e-mail already exists. Sign in, confirm your e-mail or reset your password."));

		if (registration.Status == UserRegistrationStatus.Rejected)
			return Result.Fail(new ValidationError(
				registration.Error ?? "The account could not be created."));

		// Finishing an earlier attempt
		if (existingCustomerId is not null && HasDifferentInformation(customer, customerData))
		{
			customer.UpdateInformation(customerData);
			await _customerWriteRepository.AppendEventsAndCommitAsync(customer, cancellationToken);
		}

		return Result.Ok();
	}

	private static bool HasDifferentInformation(Customer customer, CustomerData customerData) =>
		customer.Name != customerData.Name
		|| !customer.ShippingAddress.Equals(Address.FromStreetAddress(customerData.ShippingAddress))
		|| !customer.StoreCredit.Equals(StoreCredit.Create(customerData.StoreCredit));
}
