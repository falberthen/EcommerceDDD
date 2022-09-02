using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Quotes.Domain;

namespace EcommerceDDD.Application.Quotes.RemovingQuoteItem;

public record class RemoveQuoteItem(
    QuoteId QuoteId, 
    ProductId ProductId) : Command
{
    public override ValidationResult Validate()
    {
        return new RemoveQuoteItemValidator().Validate(this);
    }
}

public class RemoveQuoteItemValidator : AbstractValidator<RemoveQuoteItem>
{
    public RemoveQuoteItemValidator()
    {
        RuleFor(x => x.QuoteId.Value).NotEqual(Guid.Empty).WithMessage("QuoteId is empty.");
        RuleFor(x => x.ProductId.Value).NotEqual(Guid.Empty).WithMessage("ProductId is empty.");
    }
}
