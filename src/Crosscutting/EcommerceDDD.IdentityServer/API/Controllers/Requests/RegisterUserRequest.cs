namespace EcommerceDDD.IdentityServer.API.Controllers.Requests;

public record RegisterUserRequest
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string PasswordConfirm { get; set; }
}