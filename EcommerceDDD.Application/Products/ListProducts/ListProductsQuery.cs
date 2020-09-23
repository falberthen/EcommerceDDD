using System.Collections.Generic;
using BuildingBlocks.CQRS.QueryHandling;
using EcommerceDDD.Application.Customers.ViewModels;
using FluentValidation;
using FluentValidation.Results;

namespace EcommerceDDD.Application.Products.ListProducts
{
    public class ListProductsQuery : Query<IList<ProductViewModel>>
    {
        public string Currency { get; }

        public ListProductsQuery(string currency)
        {
            Currency = currency;
        }

        public override ValidationResult Validate()
        {
            return new ListProductsQueryValidator().Validate(this);
        }
    }

    public class ListProductsQueryValidator : AbstractValidator<ListProductsQuery>
    {
        public ListProductsQueryValidator()
        {
            RuleFor(x => x.Currency).NotEmpty().WithMessage("Currency is empty.");
        }
    }
}
