﻿namespace EcommerceDDD.CustomerManagement.Application.RegisteringCustomer;

public class RegisterCustomerHandler(
    IHttpRequester httpRequester,
    IEmailUniquenessChecker uniquenessChecker,
    IOptions<TokenIssuerSettings> tokenIssuerSettings,
    IEventStoreRepository<Customer> customerWriteRepository
) : ICommandHandler<RegisterCustomer>
{
    private readonly IHttpRequester _httpRequester = httpRequester
		?? throw new ArgumentNullException(nameof(httpRequester));
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

        var response = await CreateUserForCustomerAsync(command, customer.Id)
            ?? throw new RecordNotFoundException($"An error occurred creating the customer's user.");

        if (!response.Success)
            throw new RecordNotFoundException(response?.Message!);

        await _customerWriteRepository
            .AppendEventsAsync(customer, cancellationToken);
    }

    private async Task<IntegrationHttpResponse> CreateUserForCustomerAsync(RegisterCustomer command, CustomerId customerId)
    {
        try
        {
            var identityServerCreateUserUrl = $"{_tokenIssuerSettings.Authority}/api/accounts/register";
            var result = await _httpRequester
                .PostAsync<IntegrationHttpResponse>(identityServerCreateUserUrl,
                new RegisterUserRequest(
					customerId.Value,
                    command.Email, 
                    command.Password, 
                    command.PasswordConfirm));

            return result;
        }
        catch (Exception e)
        {
            throw new RecordNotFoundException(e.Message);
        }
    }

    private record RegisterUserRequest(
		Guid CustomerId,
		string Email, 
		string Password, 
		string PasswordConfirm);
}