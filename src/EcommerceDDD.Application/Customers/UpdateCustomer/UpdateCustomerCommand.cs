using EcommerceDDD.Application.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Application.Customers.UpdateCustomer
{
    public record class UpdateCustomerCommand : Command<Guid>
    {
        public Guid CustomerId { get; init; }
        public string Name { get; init; }

        public UpdateCustomerCommand(Guid customerId, string name)
        {
            CustomerId = customerId;
            Name = name;
        }

        public override ValidationResult Validate()
        {
            return new UpdateCustomerCommandValidation().Validate(this);            
        }
    }

    public class UpdateCustomerCommandValidation : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidation()
        {
            RuleFor(c => c.CustomerId).NotEqual(Guid.Empty).WithMessage("Customer Id is required");

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Customer Name is required")
                .Length(5, 100).WithMessage("The Name must have between 5 and 100 characters");
        }
    }
}
