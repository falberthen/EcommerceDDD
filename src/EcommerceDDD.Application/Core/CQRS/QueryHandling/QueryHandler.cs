using System;
using System.Threading;
using System.Threading.Tasks;

namespace EcommerceDDD.Application.Core.CQRS.QueryHandling;

/// <summary>
/// Interface for Query Handler implementation
/// </summary>
public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
    where TQuery : IQuery<TResult> {}

/// <summary>
/// Abstract class to be inherited by Query Handlers
/// </summary>
/// <typeparam name="TQuery"></typeparam>
/// <typeparam name="TResult"></typeparam>
public abstract class QueryHandler<TQuery, TResult> : IQueryHandler<TQuery, QueryHandlerResult<TResult>>
    where TQuery : IQuery<QueryHandlerResult<TResult>>
{
    /// To override
    public abstract Task<TResult> ExecuteQuery(TQuery query, CancellationToken cancellationToken);

    /// MediatR Handle implementation
    public async Task<QueryHandlerResult<TResult>> Handle(TQuery query, CancellationToken cancellationToken)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        QueryHandlerResult<TResult> result = new QueryHandlerResult<TResult>(query);

        try
        {
            if (result.ValidationResult.IsValid)
                result.Result = await ExecuteQuery(query, cancellationToken);
        }
        catch (Exception) { throw; }

        return result;
    }
}