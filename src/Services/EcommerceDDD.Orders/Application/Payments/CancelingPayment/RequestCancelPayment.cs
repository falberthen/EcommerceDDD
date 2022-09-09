using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Orders.Application.Payments.CancelingPayment;

public record class RequestCancelPayment(
    PaymentId PaymentId,
    PaymentCancellationReason PaymentCancellationReason) : Command
{
    public override ValidationResult Validate()
    {
        return new RequestCancelPaymentValidator().Validate(this);
    }
}

public class RequestCancelPaymentValidator : AbstractValidator<RequestCancelPayment>
{
    public RequestCancelPaymentValidator()
    {
        RuleFor(x => x.PaymentId.Value).NotEqual(Guid.Empty).WithMessage("PaymentId is empty.");
        RuleFor(x => x.PaymentCancellationReason).NotNull().WithMessage("PaymentCancellationReason is null.");        
    }
}