using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Quotes.Domain;

namespace EcommerceDDD.Quotes.Application.OpeningQuote;

public record class OpenQuote(CustomerId CustomerId) : Command
{
    public override ValidationResult Validate()
    {
        return new CreateQuoteValidator().Validate(this);
    }
}

public class CreateQuoteValidator : AbstractValidator<OpenQuote>
{
    public CreateQuoteValidator()
    {
        RuleFor(x => x.CustomerId.Value).NotEqual(Guid.Empty).WithMessage("CustomerId is empty.");
    }
}