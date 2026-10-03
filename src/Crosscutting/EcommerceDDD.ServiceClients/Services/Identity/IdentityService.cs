namespace EcommerceDDD.ServiceClients.Services.Identity;

public class IdentityService(IdentityServerClient identityServerClient) : IIdentityService
{
    private readonly IdentityServerClient _identityServerClient = identityServerClient;

    public async Task<UserRegistrationResult> RegisterUserAsync(Guid userId, string email, string password, string passwordConfirm, CancellationToken cancellationToken)
    {
        var request = new RegisterUserRequest()
        {
            CustomerId = userId,
            Email = email,
            Password = password,
            PasswordConfirm = passwordConfirm,
        };

        try
        {
            await _identityServerClient.Api.V2.Accounts.Register
                .PostAsync(request, cancellationToken: cancellationToken);

            return new(UserRegistrationStatus.Registered);
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Conflict)
        {
            return new(UserRegistrationStatus.EmailTaken);
        }
        catch (IdentityServer.Models.ValidationProblemDetails ex)
        {
            return new(UserRegistrationStatus.Rejected, ex.Detail);
        }
    }
}
