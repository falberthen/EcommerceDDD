using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.QueryHandling;
using EcommerceDDD.Products.Domain;

namespace EcommerceDDD.Products.Application.Products.CheckingProductStockAvailability;

public record class CheckProductStockAvailability(
    IList<ProductId> ProductIds) : Query<IList<ProductInStockViewModel>>
{
    public override ValidationResult Validate()
    {
        return new CheckProductStockAvailabilityValidator().Validate(this);
    }
}

public class CheckProductStockAvailabilityValidator : AbstractValidator<CheckProductStockAvailability>
{
    public CheckProductStockAvailabilityValidator()
    {
        RuleFor(x => x.ProductIds).NotEmpty().WithMessage("ProductIds is empty.");
    }
}
