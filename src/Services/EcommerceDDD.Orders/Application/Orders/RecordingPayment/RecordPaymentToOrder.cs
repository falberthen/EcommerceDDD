using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Orders.Application.Orders.RecordingPayment;

public record class RecordPaymentToOrder(
    PaymentId PaymentId,
    OrderId OrderId,
    Money TotalPaid) : Command
{
    public override ValidationResult Validate()
    {
        return new RecordPaymentToOrderValidator().Validate(this);
    }
}

public class RecordPaymentToOrderValidator : AbstractValidator<RecordPaymentToOrder>
{
    public RecordPaymentToOrderValidator()
    {
        RuleFor(x => x.PaymentId.Value).NotEqual(Guid.Empty).WithMessage("PaymentId is empty.");
        RuleFor(x => x.OrderId.Value).NotEqual(Guid.Empty).WithMessage("OrderId is empty.");
        RuleFor(x => x.TotalPaid).NotEmpty().WithMessage("TotalPaid is empty.");
    }
}