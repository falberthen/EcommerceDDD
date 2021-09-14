using System;
using BuildingBlocks.CQRS.CommandHandling;
using FluentValidation;
using FluentValidation.Results;

namespace EcommerceDDD.Application.Quotes.SaveQuote
{
    public class CreateQuoteCommand : Command<Guid>
    {
        public Guid CustomerId { get; protected set; }
        public ProductDto Product { get; protected set; }

        public CreateQuoteCommand(Guid customerId, ProductDto product)
        {
            CustomerId = customerId;
            Product = product;
        }

        public override ValidationResult Validate()
        {
            return new CreateQuoteCommandValidator().Validate(this);
        }
    }

    public class CreateQuoteCommandValidator : AbstractValidator<CreateQuoteCommand>
    {
        public CreateQuoteCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEqual(Guid.Empty).WithMessage("CustomerId is empty.");
            RuleFor(x => x.Product).NotNull().WithMessage("Product is empty.");
        }
    }
}
