namespace EcommerceDDD.IdentityServer.Models;

public class UserRegisteredResult
{
	public string UserId { get; set; } = default!;
	public bool Succeeded { get; set; }
}
