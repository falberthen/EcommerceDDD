using System;
using EcommerceDDD.Application.Core.CQRS.CommandHandling;
using FluentValidation;
using FluentValidation.Results;

namespace EcommerceDDD.Application.Payments
{
    public class MakePaymentCommand : Command<Guid>
    {
        public Guid PaymentId { get; private set; }

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
}
