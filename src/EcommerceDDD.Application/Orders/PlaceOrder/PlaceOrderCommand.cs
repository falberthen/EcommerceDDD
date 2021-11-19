using EcommerceDDD.Domain.SharedKernel;
using EcommerceDDD.Application.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Application.Orders.PlaceOrder
{
    public record class PlaceOrderCommand : Command<Guid>
    {
        public Guid QuoteId { get; init; }
        public Guid CustomerId { get; init; }
        public string Currency { get; init; }

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
