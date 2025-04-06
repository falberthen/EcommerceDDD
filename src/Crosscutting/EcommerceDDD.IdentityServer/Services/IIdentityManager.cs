namespace EcommerceDDD.IdentityServer.Services;

public interface IIdentityManager
{
	Task<LoginResult> AuthUserByCredentials(LoginRequest request);
	Task<UserRegisteredResult> RegisterNewUser(RegisterUserRequest request);
}