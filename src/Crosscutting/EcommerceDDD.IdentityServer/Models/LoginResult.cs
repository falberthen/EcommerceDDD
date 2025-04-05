namespace EcommerceDDD.IdentityServer.Models;

public class LoginResult
{
	public string? AccessToken { get; set; }
	public string? RefreshToken { get; set; }
	public string? IdentityToken { get; set; }
	public string? ErrorDescription { get; set; }
}
