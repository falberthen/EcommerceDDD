using FluentValidation.Results;
using MediatR;

namespace EcommerceDDD.Application.Core.CQRS.QueryHandling
{
    /// <summary>
    /// Interface for Query implementation
    /// </summary>
    public interface IQuery<out TResult> : IRequest<TResult>
    {
        public abstract ValidationResult Validate();
    }

    /// <summary>
    /// Abstract class to be inherited by Queries
    /// </summary>
    public abstract class Query<TResult> : IQuery<QueryHandlerResult<TResult>>
    {
        public ValidationResult ValidationResult { get; set; }

        public virtual ValidationResult Validate()
        {
            return ValidationResult;
        }
    }
}