using BuildingBlocks.CQRS.QueryHandling;
using EcommerceDDD.Application.Customers.ViewModels;
using FluentValidation;
using FluentValidation.Results;

namespace EcommerceDDD.Application.Customers.AuthenticateCustomer
{
    public class AuthenticateCustomerQuery : Query<CustomerViewModel>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public AuthenticateCustomerQuery(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public override ValidationResult Validate()
        {
            return new AuthenticateCustomerQueryValidator().Validate(this);
        }
    }

    public class AuthenticateCustomerQueryValidator : AbstractValidator<AuthenticateCustomerQuery>
    {
        public AuthenticateCustomerQueryValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is empty.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Password is empty.");
        }
    }
}
