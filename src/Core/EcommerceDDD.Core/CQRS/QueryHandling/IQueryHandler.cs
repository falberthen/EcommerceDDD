namespace EcommerceDDD.Core.CQRS.QueryHandling;

public interface IQueryHandler<in TQuery, TResponse>: IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse> {}