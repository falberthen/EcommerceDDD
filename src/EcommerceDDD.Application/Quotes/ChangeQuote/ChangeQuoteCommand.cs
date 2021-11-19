using EcommerceDDD.Application.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Application.Quotes.ChangeQuote;

public record class ChangeQuoteCommand : Command<Guid>
{
    public Guid QuoteId { get; init; }
    public ProductDto Product { get; init; }

    public ChangeQuoteCommand(Guid quoteId, ProductDto product)
    {
        QuoteId = quoteId;
        Product = product;
    }

    public override ValidationResult Validate()
    {
        return new ChangeQuoteCommandValidator().Validate(this);
    }
}

public class ChangeQuoteCommandValidator : AbstractValidator<ChangeQuoteCommand>
{
    public ChangeQuoteCommandValidator()
    {
        RuleFor(x => x.QuoteId).NotEqual(Guid.Empty).WithMessage("QuoteId is empty.");
        RuleFor(x => x.Product).NotNull().WithMessage("Product is empty.");
    }
}
