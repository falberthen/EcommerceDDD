using System.ComponentModel.DataAnnotations;

namespace EcommerceDDD.Application.Customers.RegisterCustomer;

public record class RegisterCustomerRequest
{        
    [Required(ErrorMessage = "The {0} field is required.")]
    [StringLength(100, ErrorMessage = "The {0} field must be between {2} and {1} characters", MinimumLength = 2)]
    public string Name { get; init; }

    [Required(ErrorMessage = "The {0} field is required.")]
    [EmailAddress(ErrorMessage = "The field {0} is in an invalid format")]
    public string Email { get; init; }

    [Required(ErrorMessage = "The {0} field is required.")]
    [StringLength(100, ErrorMessage = "The {0} field must be between {2} and {1} characters", MinimumLength = 5)]
    public string Password { get; init; }

    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string PasswordConfirm { get; init; }
}