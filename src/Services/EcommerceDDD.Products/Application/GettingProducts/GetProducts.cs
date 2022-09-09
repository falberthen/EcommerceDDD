using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.QueryHandling;
using EcommerceDDD.Products.Domain;

namespace EcommerceDDD.Products.Application.Products.GettingProducts;

public record class GetProducts(
    string CurrencyCode,
    IList<ProductId> ProductIds) : Query<IList<ProductViewModel>>
{
    public override ValidationResult Validate()
    {
        return new ListProductsQueryValidator().Validate(this);
    }
}

public class ListProductsQueryValidator : AbstractValidator<GetProducts>
{
    public ListProductsQueryValidator()
    {
        RuleFor(x => x.CurrencyCode).NotEmpty().WithMessage("Currency code is empty.");
    }
}
