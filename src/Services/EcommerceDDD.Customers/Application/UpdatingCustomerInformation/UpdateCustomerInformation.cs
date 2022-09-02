using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Customers.Domain;

namespace EcommerceDDD.Customers.Api.Application.UpdatingCustomerInformation;

public record class UpdateCustomerInformation(
    CustomerId CustomerId,
    string Name,
    string Address) : Command
{
    public override ValidationResult Validate()
    {
        return new UpdateCustomerCommandValidator().Validate(this);
    }
}

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerInformation>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.CustomerId.Value)
        .NotEqual(Guid.Empty).WithMessage("CustomerId is empty.");

        RuleFor(c => c.Name)
        .NotEmpty().WithMessage("Name is empty.")
        .Length(2, 100).WithMessage("The Name must have between 2 and 100 characters.");

        RuleFor(c => c.Address)
        .NotEmpty().WithMessage("Address is empty.");
    }
}