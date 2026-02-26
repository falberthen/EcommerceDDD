using EcommerceDDD.ServiceClients.IdentityServer;
using EcommerceDDD.ServiceClients.IdentityServer.Models;

namespace EcommerceDDD.ServiceClients.Services.Identity;

public class IdentityService(IdentityServerClient identityServerClient) : IIdentityService
{
    private readonly IdentityServerClient _identityServerClient = identityServerClient;

    public async Task RegisterUserAsync(Guid customerId, string email, string password, string passwordConfirm, CancellationToken cancellationToken)
    {
        var request = new RegisterUserRequest()
        {
            CustomerId = customerId,
            Email = email,
            Password = password,
            PasswordConfirm = passwordConfirm,
        };

        await _identityServerClient.Api.V2.Accounts.Register
            .PostAsync(request, cancellationToken: cancellationToken);
    }
}
