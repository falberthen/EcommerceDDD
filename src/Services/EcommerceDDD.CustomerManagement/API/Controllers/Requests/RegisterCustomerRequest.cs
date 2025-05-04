namespace EcommerceDDD.CustomerManagement.API.Controllers.Requests;

public record class RegisterCustomerRequest
{
    [Required(ErrorMessage = "The {0} field is required.")]
    [EmailAddress(ErrorMessage = "The field {0} is in an invalid format")]
    public string Email { get; init; }

    [Required(ErrorMessage = "The {0} field is required.")]
    [StringLength(100, ErrorMessage = "The {0} field must be between {2} and {1} characters", MinimumLength = 5)]
    public string Password { get; init; }

    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string PasswordConfirm { get; init; }

    [Required(ErrorMessage = "The {0} field is required.")]
    [StringLength(100, ErrorMessage = "The {0} field must be between {2} and {1} characters", MinimumLength = 2)]
    public string Name { get; init; }

    [StringLength(100, ErrorMessage = "The {0} field must be between {2} and {1} characters", MinimumLength = 5)]
    public string ShippingAddress { get; init; }

    [Required(ErrorMessage = "The {0} field is required.")]
	[Range(0.01, double.MaxValue, ErrorMessage = "The {0} must be greater than zero.")]
	public decimal CreditLimit { get; init; }
}