using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Customers.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;
using Microsoft.Extensions.Options;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.Core.Infrastructure.Http;

namespace EcommerceDDD.Customers.Api.Application.RegisteringCustomer;

public class RegisterCustomerHandler : CommandHandler<RegisterCustomer>
{
    private readonly IHttpRequester _httpRequester;
    private readonly ICustomerUniquenessChecker _uniquenessChecker;
    private readonly TokenIssuerSettings _tokenIssuerSettings;
    private readonly IEventStoreRepository<Customer> _customerWriteRepository;

    public RegisterCustomerHandler(
        IHttpRequester httpRequester,
        ICustomerUniquenessChecker uniquenessChecker,
        IOptions<TokenIssuerSettings> tokenIssuerSettings,        
        IEventStoreRepository<Customer> customerWriteRepository)
    {
        _httpRequester = httpRequester;
        _uniquenessChecker = uniquenessChecker;
        _tokenIssuerSettings = tokenIssuerSettings.Value;
        _customerWriteRepository = customerWriteRepository;
    }

    public override async Task Handle(RegisterCustomer command, CancellationToken cancellationToken)
    {
        var customer = Customer.CreateNew(
            command.Email,
            command.Name,
            command.Address,
            command.AvailableCreditLimit,
            _uniquenessChecker);
        
        var response = await CreateUserForCustomer(command);
        if (!response.IsSuccessStatusCode)
            throw new ApplicationException("Cannot create customer's user.");

        await _customerWriteRepository
            .AppendEventsAsync(customer);        
    }

    private async Task<HttpResponseMessage> CreateUserForCustomer(RegisterCustomer command)
    {
        try
        {
            var identityServerCreateUserUrl = $"{_tokenIssuerSettings.Authority}/api/accounts/register";
            var result = await _httpRequester
                .PostAsync<HttpResponseMessage>(identityServerCreateUserUrl,
                new RegisterUserRequest(
                    command.Email, 
                    command.Password, 
                    command.PasswordConfirm));

            return result;
        }
        catch (Exception e)
        {
            throw new ApplicationDataException(e.Message);
        }
    }

    private record RegisterUserRequest(string Email, string Password, string PasswordConfirm);
}