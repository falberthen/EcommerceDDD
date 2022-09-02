using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.QueryHandling;

namespace EcommerceDDD.Quotes.Application.GettingOpenQuote;

public record class GetOpenQuote(Guid CustomerId, string CurrencyCode) : Query<QuoteViewModel>
{
    public override ValidationResult Validate()
    {
        return new GetOpenQuoteQueryValidator().Validate(this);
    }
}

public class GetOpenQuoteQueryValidator : AbstractValidator<GetOpenQuote>
{
    public GetOpenQuoteQueryValidator()
    {
        RuleFor(x => x.CustomerId).NotEqual(Guid.Empty).WithMessage("Customer is empty.");
        RuleFor(x => x.CurrencyCode).NotEmpty().WithMessage("Currency code is empty.");
    }
}