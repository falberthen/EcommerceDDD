using EcommerceDDD.Application.Core.CQRS.QueryHandling;

namespace EcommerceDDD.Application.Quotes.GetQuoteDetails;

public record class GetQuoteDetailsQuery : Query<QuoteDetailsViewModel>
{
    public Guid QuoteId { get; init; }
    public string Currency { get; init; }

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