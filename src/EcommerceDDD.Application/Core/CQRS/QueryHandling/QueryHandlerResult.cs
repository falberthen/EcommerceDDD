using FluentValidation.Results;

namespace EcommerceDDD.Application.Core.CQRS.QueryHandling
{
    /// <summary>
    /// QueryHandler result class
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class QueryHandlerResult<TResult>
    {
        public ValidationResult ValidationResult { get; }

        public TResult Result { get; set; }

        public QueryHandlerResult(IQuery<QueryHandlerResult<TResult>> query)
        {
            ValidationResult = query.Validate();
        }
    }
}