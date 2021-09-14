using System;
using BuildingBlocks.CQRS.CommandHandling;
using FluentValidation;
using FluentValidation.Results;

namespace EcommerceDDD.Application.Quotes.SaveQuote
{
    public class ChangeQuoteCommand : Command<Guid>
    {
        public Guid QuoteId { get; protected set; }
        public ProductDto Product { get; protected set; }

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
}
