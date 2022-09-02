using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Orders.Application.Quotes;
using EcommerceDDD.Orders.Domain;

namespace EcommerceDDD.Orders.Application.Orders.PlacingOrder;

public record class PlaceOrder(
    OrderId OrderId,
    ConfirmedQuote ConfirmedQuote) : Command
{
    public override ValidationResult Validate()
    {
        return new PlaceOrderValidator().Validate(this);
    }
}

public class PlaceOrderValidator : AbstractValidator<PlaceOrder>
{
    public PlaceOrderValidator()
    {
        RuleFor(x => x.OrderId.Value).NotEqual(Guid.Empty).WithMessage("OrderId is empty.");
        RuleFor(x => x.ConfirmedQuote.CustomerId.Value).NotEqual(Guid.Empty).WithMessage("QuoteId is empty.");
        RuleFor(x => x.ConfirmedQuote.Id.Value).NotEqual(Guid.Empty).WithMessage("QuoteId is empty.");
        RuleFor(x => x.ConfirmedQuote.Items).NotNull().WithMessage("QuoteItems is null.");
        RuleFor(x => x.ConfirmedQuote.Items).NotEmpty().WithMessage("QuoteItems is empty.");
    }
}