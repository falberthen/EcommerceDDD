using System;
using EcommerceDDD.Domain.Shared;
using FluentValidation;
using FluentValidation.Results;
using BuildingBlocks.CQRS.CommandHandling;

namespace EcommerceDDD.Application.Orders.PlaceOrder
{
    public class PlaceOrderCommand : Command<Guid>
    {
        public Guid CartId { get; private set; }
        public Guid CustomerId { get; private set; }
        public string Currency { get; private set; }

        public PlaceOrderCommand(Guid cartId, Guid customerId, string currency)
        {
            CartId = cartId;
            CustomerId = customerId;
            Currency = currency;
        }

        public override ValidationResult Validate()
        {
            return new PlaceOrderCommandValidator().Validate(this);
        }
    }

    public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
    {
        public PlaceOrderCommandValidator()
        {
            RuleFor(x => x.CartId).NotEqual(Guid.Empty).WithMessage("CartId is empty.");
            RuleFor(x => x.CustomerId).NotEqual(Guid.Empty).WithMessage("CustomerId is empty.");

            this.RuleFor(x => x.Currency).Must(c => Currency.SupportedCurrencies().Contains(c))
                .WithMessage("At least one product has invalid currency.");
        }
    }
}
