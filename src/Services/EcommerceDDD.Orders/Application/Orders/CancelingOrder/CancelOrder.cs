using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Orders.Application.Orders.CancelingOrder;

public record class CancelOrder(    
    OrderId OrderId,
    OrderCancellationReason CancellationReason) : Command
{
    public override ValidationResult Validate()
    {
        return new CancelOrderValidator().Validate(this);
    }
}

public class CancelOrderValidator : AbstractValidator<CancelOrder>
{
    public CancelOrderValidator()
    {
        RuleFor(x => x.CancellationReason).NotNull().WithMessage("CancellationReason is null.");
        RuleFor(x => x.OrderId.Value).NotEqual(Guid.Empty).WithMessage("OrderId is empty.");
    }
}