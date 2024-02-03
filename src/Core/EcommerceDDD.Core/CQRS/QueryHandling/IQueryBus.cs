namespace EcommerceDDD.Core.CQRS.QueryHandling;

public interface IQueryBus
{
    Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken);
}
