using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Quotes.Domain;

namespace EcommerceDDD.Quotes.Application.AddingQuoteItem;

public record class AddQuoteItem(
    QuoteId QuoteId,
    ProductId ProductId,
    int Quantity,
    string CurrencyCode) : Command
{
    public override ValidationResult Validate()
    {
        return new ChangeQuoteValidator().Validate(this);
    }
}

public class ChangeQuoteValidator : AbstractValidator<AddQuoteItem>
{
    public ChangeQuoteValidator()
    {
        RuleFor(x => x.QuoteId.Value).NotEqual(Guid.Empty).WithMessage("QuoteId is empty.");
        RuleFor(x => x.ProductId.Value).NotEqual(Guid.Empty).WithMessage("ProductId is empty.");
        RuleFor(x => x.Quantity).NotEmpty().WithMessage("Quantity is empty.");
        RuleFor(x => x.CurrencyCode).NotEmpty().WithMessage("Currency code is empty.");
    }
}
