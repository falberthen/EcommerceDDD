using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Core.Testing;

public record class DummyCommand(DummyAggregateId Id) : Command
{
    public override ValidationResult Validate()
    {
        return new DummyCommandValidator().Validate(this);
    }
}

public class DummyCommandValidator : AbstractValidator<DummyCommand>
{
    public DummyCommandValidator()
    {
        RuleFor(x => x.Id.Value).NotEqual(Guid.Empty).WithMessage("Id is empty.");
    }
}