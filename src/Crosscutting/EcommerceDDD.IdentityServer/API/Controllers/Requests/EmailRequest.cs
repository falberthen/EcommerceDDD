namespace EcommerceDDD.IdentityServer.API.Controllers.Requests;

public record EmailRequest
{
	[Required]
	[EmailAddress]
	public string Email { get; set; }
}
