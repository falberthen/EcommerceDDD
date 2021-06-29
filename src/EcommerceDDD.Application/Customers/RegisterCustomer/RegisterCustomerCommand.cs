using System;
using FluentValidation.Results;
using BuildingBlocks.CQRS.CommandHandling;
using FluentValidation;

namespace EcommerceDDD.Application.Customers.RegisterCustomer
{
    public class RegisterCustomerCommand : Command<Guid>
    {
        public string Email { get; protected set; }
        public string Password { get; protected set; }
        public string PasswordConfirm { get; protected set; }
        public string Name { get; protected set; }

        public RegisterCustomerCommand(string email, string name, string password, string passwordConfirm)
        {
            Name = name;
            Email = email;
            Password = password;
            PasswordConfirm = passwordConfirm;
        }

        public override ValidationResult Validate()
        {
            return new RegisterCustomerCommandValidator().Validate(this);
        }
    }

    public class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomerCommand>
    {
        public RegisterCustomerCommandValidator()
        {
            RuleFor(c => c.Email)
            .NotEmpty().WithMessage("Email is empty.")
            .Length(5, 100).WithMessage("The Email must have between 5 and 100 characters.");

            RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is empty.")
            .Length(2, 100).WithMessage("The Name must have between 2 and 100 characters.");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is empty.");
            RuleFor(x => x.PasswordConfirm).NotEmpty().WithMessage("PasswordConfirm is empty.");
        }
    }
}
