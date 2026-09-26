namespace EcommerceDDD.ServiceClients.Services.Identity;

public interface IIdentityService
{
    /// <summary>
    /// Creates the user under <paramref name="userId"/>. Idempotent: replaying the same request
    /// reports <see cref="UserRegistrationStatus.Registered"/> again. Transport failures throw.
    /// </summary>
    Task<UserRegistrationResult> RegisterUserAsync(Guid userId, string email, string password, string passwordConfirm, CancellationToken cancellationToken);
}
