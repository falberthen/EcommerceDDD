using System;
using BuildingBlocks.CQRS.CommandHandling;
using FluentValidation;
using FluentValidation.Results;

namespace EcommerceDDD.Application.Carts.CreateCart
{
    public class SaveCartCommand : Command<Guid>
    {
        public Guid CustomerId { get; protected set; }
        public ProductDto Product { get; protected set; }

        public SaveCartCommand(Guid customerId, ProductDto product)
        {
            CustomerId = customerId;
            Product = product;
        }

        public override ValidationResult Validate()
        {
            return new CreateCartCommandValidator().Validate(this);
        }
    }

    public class CreateCartCommandValidator : AbstractValidator<SaveCartCommand>
    {
        public CreateCartCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEqual(Guid.Empty).WithMessage("CustomerId is empty.");
            RuleFor(x => x.Product).NotNull().WithMessage("Product is empty.");
        }
    }
}
