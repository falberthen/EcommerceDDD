using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Orders.Application.Payments.RequestingPayment;

public record class RequestPayment(
    CustomerId CustomerId,
    OrderId OrderId,
    Money TotalPrice,
    string CurrencyCode) : Command
{
    public override ValidationResult Validate()
    {
        return new RequestPaymentValidator().Validate(this);
    }
}

public class RequestPaymentValidator : AbstractValidator<RequestPayment>
{
    public RequestPaymentValidator()
    {
        RuleFor(x => x.CustomerId.Value).NotEqual(Guid.Empty).WithMessage("CustomerId is empty.");
        RuleFor(x => x.OrderId.Value).NotEqual(Guid.Empty).WithMessage("OrderId is empty.");
        RuleFor(x => x.TotalPrice).NotEmpty().WithMessage("TotalPrice is empty.");
        RuleFor(x => x.CurrencyCode).NotEmpty().WithMessage("CurrencyCode is empty.");
    }
}