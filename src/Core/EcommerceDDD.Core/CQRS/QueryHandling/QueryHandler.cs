using MediatR;
using System.Threading;

namespace EcommerceDDD.Core.CQRS.QueryHandling;

public abstract class QueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public abstract Task<TResult> Handle(TQuery query, CancellationToken cancellationToken);

    async Task<TResult> IRequestHandler<TQuery, TResult>.Handle(TQuery query, CancellationToken cancellationToken)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        TResult result = default;

        try
        {
            if (query.ValidationResult.IsValid)
                result = await Handle(query, cancellationToken);
        }
        catch (Exception) 
        { 
            throw; 
        }

        return result;
    }
}
