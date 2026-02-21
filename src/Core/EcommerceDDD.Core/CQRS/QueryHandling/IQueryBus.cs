namespace EcommerceDDD.Core.CQRS.QueryHandling;

public interface IQueryBus
{
	Task<Result<TResponse>> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken);
}
