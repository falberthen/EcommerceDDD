using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Quotes.Domain;

namespace EcommerceDDD.Application.Quotes.CancelingQuote;

public record class CancelQuote(QuoteId QuoteId) : Command
{
    public override ValidationResult Validate()
    {
        return new CancelQuoteValidator().Validate(this);
    }
}

public class CancelQuoteValidator : AbstractValidator<CancelQuote>
{
    public CancelQuoteValidator()
    {
        RuleFor(x => x.QuoteId.Value).NotEqual(Guid.Empty).WithMessage("QuoteId is empty.");
    }
}
