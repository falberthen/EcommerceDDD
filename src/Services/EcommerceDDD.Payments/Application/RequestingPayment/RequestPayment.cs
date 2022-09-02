using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Payments.Domain;

namespace EcommerceDDD.Payments.Application.RequestingPayment;

public record class RequestPayment(
    CustomerId CustomerId,
    OrderId OrderId,
    Money TotalAmount,
    Currency Currency) : Command
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
        RuleFor(x => x.TotalAmount).NotEmpty().WithMessage("TotalAmount is empty.");
        RuleFor(x => x.Currency).NotEmpty().WithMessage("Currency is empty.");
    }
}