using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace EcommerceDDD.Application.Customers.RegisterCustomer
{
    public class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomerCommand> 
    {
        public RegisterCustomerCommandValidator()
        {
            RuleFor(c => c.Email)
            .NotEmpty().WithMessage("Customer Email is empty.")
            .Length(5, 100).WithMessage("The Email must have between 5 and 100 characters.");

            RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Customer Name is empty.")
            .Length(2, 100).WithMessage("The Name must have between 2 and 100 characters.");
        }
    }
}
