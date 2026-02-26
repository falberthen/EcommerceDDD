namespace EcommerceDDD.ServiceClients.Services.Identity;

public interface IIdentityService
{
    Task RegisterUserAsync(Guid customerId, string email, string password, string passwordConfirm, CancellationToken cancellationToken);
}
