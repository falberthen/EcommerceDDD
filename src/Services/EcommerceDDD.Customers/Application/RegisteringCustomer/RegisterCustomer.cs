using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Customers.Api.Application.RegisteringCustomer;

public record class RegisterCustomer(
    string Email, 
    string Password, 
    string PasswordConfirm, 
    string Name, 
    string Address,
    decimal AvailableCreditLimit) : Command
{
    public override ValidationResult Validate()
    {
        return new RegisterCustomerCommandValidator().Validate(this);
    }
}

public class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomer>
{
    public RegisterCustomerCommandValidator()
    {
        RuleFor(c => c.Email)
        .NotEmpty().WithMessage("Email is empty.")
        .Length(5, 100).WithMessage("The Email must have between 5 and 100 characters.");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is empty.");
        RuleFor(x => x.PasswordConfirm).NotEmpty().WithMessage("PasswordConfirm is empty.");
        RuleFor(x => x).Custom((x, context) =>
        {
            if (x.Password != x.PasswordConfirm)
            {
                context.AddFailure(nameof(x.Password), "Passwords should match.");
            }
        });

        RuleFor(c => c.Name)
        .NotEmpty().WithMessage("Name is empty.")
        .Length(2, 100).WithMessage("The Name must have between 2 and 100 characters.");

        RuleFor(c => c.Address)
        .NotEmpty().WithMessage("Address is empty.");

        RuleFor(c => c.AvailableCreditLimit)
        .GreaterThan(0).WithMessage("AvailableCreditLimit must be greater than 0.");
    }
}