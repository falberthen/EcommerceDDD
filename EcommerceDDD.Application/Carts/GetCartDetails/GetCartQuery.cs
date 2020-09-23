using System;
using BuildingBlocks.CQRS.QueryHandling;
using FluentValidation;
using FluentValidation.Results;

namespace EcommerceDDD.Application.Carts.GetCartDetails
{
    public class GetCartDetailsQuery : Query<CartDetailsViewModel>
    {
        public Guid CustomerId { get; set; }
        public string Currency { get; set; }

        public GetCartDetailsQuery(Guid customerId, string currency)
        {
            CustomerId = customerId;
            Currency = currency;
        }

        public override ValidationResult Validate()
        {
            return new GetCartDetailsQueryValidator().Validate(this);
        }
    }

    public class GetCartDetailsQueryValidator : AbstractValidator<GetCartDetailsQuery>
    {
        public GetCartDetailsQueryValidator()
        {
            RuleFor(x => x.CustomerId).NotEqual(Guid.Empty).WithMessage("CustomerId is empty.");
            RuleFor(x => x.Currency).NotEmpty().WithMessage("Currency is empty.");
        }
    }
}
