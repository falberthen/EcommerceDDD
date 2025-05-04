using EcommerceDDD.ServiceClients.ApiGateway;
using EcommerceDDD.ServiceClients.ApiGateway.Models;

namespace EcommerceDDD.CustomerManagement.Application.RegisteringCustomer;

public class RegisterCustomerHandler(
	ApiGatewayClient apiGatewayClient,
	IEmailUniquenessChecker uniquenessChecker,
	IOptions<TokenIssuerSettings> tokenIssuerSettings,
	IEventStoreRepository<Customer> customerWriteRepository
) : ICommandHandler<RegisterCustomer>
{
	private readonly ApiGatewayClient _apiGatewayClient = apiGatewayClient
		?? throw new ArgumentNullException(nameof(apiGatewayClient));
	private readonly IEmailUniquenessChecker _uniquenessChecker = uniquenessChecker
		?? throw new ArgumentNullException(nameof(uniquenessChecker));
	private readonly TokenIssuerSettings _tokenIssuerSettings = tokenIssuerSettings.Value
		?? throw new ArgumentNullException(nameof(tokenIssuerSettings));
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

		var response = await CreateUserForCustomerAsync(command, customer.Id, cancellationToken)
			?? throw new RecordNotFoundException($"An error occurred creating the customer's user.");

		if (response.Succeeded == false)
			throw new RecordNotFoundException($"An error occurred creating the customer's user.");

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

			var accountRequestBuilder = _apiGatewayClient.Api.V2.Accounts;
			var response = await accountRequestBuilder.Register
				.PostAsync(request, cancellationToken: cancellationToken);

			return response;
		}
		catch (Exception e)
		{
			throw new RecordNotFoundException(e.Message);
		}
	}
}