using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.QueryHandling;
using EcommerceDDD.Quotes.Application.GettingQuoteHistory;

namespace EcommerceDDD.Quotes.Api.Application.GettingQuoteHistory;

public record class GetQuoteEventHistory(Guid QuoteId) : Query<IList<QuoteEventHistory>>
{
    public override ValidationResult Validate()
    {
        return new GetQuoteHistoryValidator().Validate(this);
    }
}

public class GetQuoteHistoryValidator : AbstractValidator<GetQuoteEventHistory>
{
    public GetQuoteHistoryValidator()
    {
        RuleFor(x => x.QuoteId).NotEmpty().WithMessage("QuoteId is empty.");
    }
}
