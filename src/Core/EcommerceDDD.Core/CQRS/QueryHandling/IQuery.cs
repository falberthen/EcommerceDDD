using FluentValidation.Results;
using MediatR;

namespace EcommerceDDD.Core.CQRS.QueryHandling;

public interface IQuery<out TResponse>: IRequest<TResponse>
{
    public ValidationResult ValidationResult { get; init; }

    public virtual ValidationResult Validate()
    {
        return ValidationResult;
    }
}