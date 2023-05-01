namespace EcommerceDDD.Customers.Api.Application.RegisteringCustomer;

public class RegisterCustomerHandler : ICommandHandler<RegisterCustomer>
{
    private readonly IHttpRequester _httpRequester;
    private readonly IEmailUniquenessChecker _uniquenessChecker;
    private readonly TokenIssuerSettings _tokenIssuerSettings;
    private readonly IEventStoreRepository<Customer> _customerWriteRepository;

    public RegisterCustomerHandler(
        IHttpRequester httpRequester,
        IEmailUniquenessChecker uniquenessChecker,
        IOptions<TokenIssuerSettings> tokenIssuerSettings,        
        IEventStoreRepository<Customer> customerWriteRepository)
    {
        _httpRequester = httpRequester;
        _uniquenessChecker = uniquenessChecker;
        _tokenIssuerSettings = tokenIssuerSettings.Value;
        _customerWriteRepository = customerWriteRepository;
    }

    public async Task Handle(RegisterCustomer command, CancellationToken cancellationToken)
    {
        if (!_uniquenessChecker.IsUnique(command.Email))
            throw new BusinessRuleException("This e-mail is already in use.");

        var customerData = new CustomerData(
            command.Email,
            command.Name,
            command.ShippingAddress,
            command.CreditLimit);

        var customer = Customer.Create(customerData);
        var response = await CreateUserForCustomer(command);

        if (response is null)
            throw new RecordNotFoundException($"An error occurred creating the customer's user.");

        if (!response.Success)
            throw new RecordNotFoundException(response.Message);

        await _customerWriteRepository
            .AppendEventsAsync(customer);
    }

    private async Task<IntegrationHttpResponse?> CreateUserForCustomer(RegisterCustomer command)
    {
        try
        {
            var identityServerCreateUserUrl = $"{_tokenIssuerSettings.Authority}/api/accounts/register";
            var result = await _httpRequester
                .PostAsync<IntegrationHttpResponse>(identityServerCreateUserUrl,
                new RegisterUserRequest(
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

    private record RegisterUserRequest(string Email, string Password, string PasswordConfirm);
}