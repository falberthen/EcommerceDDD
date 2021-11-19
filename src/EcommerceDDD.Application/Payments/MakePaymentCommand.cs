using EcommerceDDD.Application.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Application.Payments;

public record class MakePaymentCommand : Command<Guid>
{
    public Guid PaymentId { get; init; }

    public MakePaymentCommand(Guid paymentId)
    {
        PaymentId = paymentId;
    }

    public override ValidationResult Validate()
    {
        return new MakePaymentCommandValidator().Validate(this);
    }
}

public class MakePaymentCommandValidator : AbstractValidator<MakePaymentCommand>
{
    public MakePaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEqual(Guid.Empty).WithMessage("PaymentId is empty.");
    }
}