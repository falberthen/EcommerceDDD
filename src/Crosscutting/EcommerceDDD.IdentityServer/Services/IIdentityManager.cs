namespace EcommerceDDD.IdentityServer.Services;

public interface IIdentityManager
{
	Task<LoginResult?> AuthUserByCredentials(LoginRequest request);
	Task<bool> IsAwaitingEmailConfirmation(LoginRequest request);
	Task<UserRegistration> RegisterNewUser(RegisterUserRequest request);
	Task<bool> ConfirmEmail(ConfirmEmailRequest request);
	Task ResendConfirmationLink(string email);
	Task SendPasswordResetLink(string email);
	Task<IdentityResult> ResetPassword(ResetPasswordRequest request);
}
