namespace EcommerceDDD.IdentityServer.API.Controllers.Requests;

public record ResetPasswordRequest
{
	[Required]
	public string UserId { get; set; }
	[Required]
	public string Token { get; set; }
	[Required]
	public string Password { get; set; }
	[Compare("Password", ErrorMessage = "Passwords do not match.")]
	public string PasswordConfirm { get; set; }
}
