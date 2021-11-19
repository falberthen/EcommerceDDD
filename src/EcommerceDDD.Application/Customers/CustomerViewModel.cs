using System.Text.Json.Serialization;

namespace EcommerceDDD.Application.Customers;

public record class CustomerViewModel
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Token { get; set; }

    public ValidationResult ValidationResult { get; set; } = new ValidationResult();

    [JsonIgnore]
    public bool LoginSucceeded { get; set; }
}