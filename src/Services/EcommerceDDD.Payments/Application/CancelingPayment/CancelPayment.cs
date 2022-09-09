using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Payments.Domain;

namespace EcommerceDDD.Payments.Application.CancelingPayment;

public record class CancelPayment(
    PaymentId PaymentId,
    PaymentCancellationReason PaymentCancellationReason) : Command
{
    public override ValidationResult Validate()
    {
        return new CancelPaymentValidator().Validate(this);
    }
}

public class CancelPaymentValidator : AbstractValidator<CancelPayment>
{
    public CancelPaymentValidator()
    {
        RuleFor(x => x.PaymentId.Value).NotEqual(Guid.Empty).WithMessage("PaymentId is empty.");
        RuleFor(x => x.PaymentCancellationReason).NotNull().WithMessage("PaymentCancellationReason is null.");
    }
}