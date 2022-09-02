using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Shipments.Domain;

namespace EcommerceDDD.Shipments.Application.ShippingPackage;

public record class ShipPackage(
    OrderId OrderId,
    IReadOnlyList<ProductItem> ProductItems) : Command
{
    public override ValidationResult Validate()
    {
        return new ShipOrderPackageValidator().Validate(this);
    }
}

public class ShipOrderPackageValidator : AbstractValidator<ShipPackage>
{
    public ShipOrderPackageValidator()
    {
        RuleFor(x => x.OrderId.Value).NotEqual(Guid.Empty).WithMessage("OrderId is empty.");
        RuleFor(x => x.ProductItems).Empty().WithMessage("ProductItems is empty.");
    }
}