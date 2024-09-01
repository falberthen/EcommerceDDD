namespace EcommerceDDD.IdentityServer.Services;

public interface IIdentityManager
{
	Task<TokenResponse> AuthUserByCredentials(LoginRequest request);
	Task<IdentityResult> RegisterNewUser(RegisterUserRequest request);
}
