using System;
using BuildingBlocks.CQRS.QueryHandling;
using FluentValidation;
using FluentValidation.Results;

namespace EcommerceDDD.Application.Quotes.GetCurrentQuote
{
    public class GetCurrentQuoteQuery : Query<Guid>
    {
        public Guid CustomerId { get; set; }

        public GetCurrentQuoteQuery(Guid customerId)
        {
            CustomerId = customerId;
        }

        public override ValidationResult Validate()
        {
            return new GetCurrentQuoteQueryValidator().Validate(this);
        }
    }

    public class GetCurrentQuoteQueryValidator : AbstractValidator<GetCurrentQuoteQuery>
    {
        public GetCurrentQuoteQueryValidator()
        {
            RuleFor(x => x.CustomerId).NotEqual(Guid.Empty).WithMessage("Customer is empty.");
        }
    }
}
