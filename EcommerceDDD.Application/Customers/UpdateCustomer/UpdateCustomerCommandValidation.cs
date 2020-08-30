using FluentValidation;

namespace EcommerceDDD.Application.Customers.UpdateCustomer
{
    public class UpdateCustomerCommandValidation : AbstractValidator<UpdateCustomerCommand> 
    {
        public UpdateCustomerCommandValidation()
        {
            RuleFor(c => c.CustomerId)
                .NotEmpty().WithMessage("Customer Id is required");

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Customer Name is required")
                .Length(5, 100).WithMessage("The Name must have between 5 and 100 characters");
        }
    }
}
