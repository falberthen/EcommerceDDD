namespace EcommerceDDD.Core.CQRS.QueryHandling;

public interface IQueryHandler<TQuery, TResponse>
	where TQuery : IQuery<TResponse>
{
	Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken);
}