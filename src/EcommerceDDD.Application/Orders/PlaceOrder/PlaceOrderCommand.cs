using System;
using FluentValidation;
using FluentValidation.Results;
using BuildingBlocks.CQRS.CommandHandling;
using EcommerceDDD.Domain.SharedKernel;

namespace EcommerceDDD.Application.Orders.PlaceOrder
{
    public class PlaceOrderCommand : Command<Guid>
    {
        public Guid QuoteId { get; private set; }
        public Guid CustomerId { get; private set; }
        public string Currency { get; private set; }

        public PlaceOrderCommand(Guid quoteId, Guid customerId, string currency)
        {
            QuoteId = quoteId;
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
            RuleFor(x => x.QuoteId).NotEqual(Guid.Empty).WithMessage("Quote is empty.");
            RuleFor(x => x.CustomerId).NotEqual(Guid.Empty).WithMessage("CustomerId is empty.");

            this.RuleFor(x => x.Currency).Must(c => Currency.SupportedCurrencies().Contains(c))
                .WithMessage("At least one product has invalid currency.");
        }
    }
}
