using FluentValidation.Results;

namespace EcommerceDDD.Core.CQRS.QueryHandling;

public abstract record class Query<TResult> : IQuery<TResult>
{
    public ValidationResult ValidationResult { get; init; } = new();

    public virtual ValidationResult Validate()
    {
        return ValidationResult;
    }
}