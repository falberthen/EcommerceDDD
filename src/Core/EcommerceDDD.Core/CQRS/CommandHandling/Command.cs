using FluentValidation.Results;

namespace EcommerceDDD.Core.CQRS.CommandHandling;

public abstract record class Command : ICommand
{
    public ValidationResult ValidationResult { get; init; } = new();

    public virtual ValidationResult Validate()
    {
        return ValidationResult;
    }
}