namespace EcommerceDDD.IdentityServer.API.Controllers.Requests;

public record ConfirmEmailRequest
{
	[Required]
	public string UserId { get; set; }

	[Required]
	public string Token { get; set; }
}
