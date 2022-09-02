using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Quotes.Domain;

namespace EcommerceDDD.Quotes.Application.PlacingOrderFromQuote;

public record class PlaceOrderFromQuote(QuoteId QuoteId, string CurrencyCode) : Command
{
    public override ValidationResult Validate()
    {
        return new ConfirmQuoteValidator().Validate(this);
    }
}

public class ConfirmQuoteValidator : AbstractValidator<PlaceOrderFromQuote>
{
    public ConfirmQuoteValidator()
    {
        RuleFor(x => x.CurrencyCode).NotEmpty().WithMessage("CurrencyCode is empty.");
        RuleFor(x => x.QuoteId.Value).NotEqual(Guid.Empty).WithMessage("QuoteId is empty.");
    }
}