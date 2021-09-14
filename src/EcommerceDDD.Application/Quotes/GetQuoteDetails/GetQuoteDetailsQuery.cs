using System;
using BuildingBlocks.CQRS.QueryHandling;
using FluentValidation;
using FluentValidation.Results;

namespace EcommerceDDD.Application.Quotes.GetQuoteDetails
{
    public class GetQuoteDetailsQuery : Query<QuoteDetailsViewModel>
    {
        public Guid QuoteId { get; set; }
        public string Currency { get; set; }

        public GetQuoteDetailsQuery(Guid quoteId, string currency)
        {
            QuoteId = quoteId;
            Currency = currency;
        }

        public override ValidationResult Validate()
        {
            return new GetQuotetDetailsQueryValidator().Validate(this);
        }
    }

    public class GetQuotetDetailsQueryValidator : AbstractValidator<GetQuoteDetailsQuery>
    {
        public GetQuotetDetailsQueryValidator()
        {
            RuleFor(x => x.QuoteId).NotEqual(Guid.Empty).WithMessage("QuoteId is empty.");
            RuleFor(x => x.Currency).NotEmpty().WithMessage("Currency is empty.");
        }
    }
}
