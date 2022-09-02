using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Orders.Application.Orders.CompletingOrder;

public record class CompleteOrder(    
    OrderId OrderId) : Command
{
    public override ValidationResult Validate()
    {
        return new CompleteOrderValidator().Validate(this);
    }
}

public class CompleteOrderValidator : AbstractValidator<CompleteOrder>
{
    public CompleteOrderValidator()
    {
        RuleFor(x => x.OrderId.Value).NotEqual(Guid.Empty).WithMessage("OrderId is empty.");
    }
}