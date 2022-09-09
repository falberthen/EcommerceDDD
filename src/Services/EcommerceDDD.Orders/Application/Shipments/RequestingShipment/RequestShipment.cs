using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Orders.Application.Shipments.RequestingShipment;

public record class RequestShipment(
    OrderId OrderId,
    IReadOnlyList<OrderLine> OrderLines) : Command
{
    public override ValidationResult Validate()
    {
        return new RequestShipmentValidator().Validate(this);
    }
}

public class RequestShipmentValidator : AbstractValidator<RequestShipment>
{
    public RequestShipmentValidator()
    {
        RuleFor(x => x.OrderId.Value).NotEqual(Guid.Empty).WithMessage("OrderId is empty.");
        RuleFor(x => x.OrderLines).NotNull().WithMessage("OrderLines is null.");
    }
}