namespace EcommerceDDD.Core.Infrastructure.CQRS;

public class QueryBus(IMediator mediator, ILogger<CommandBus> logger) : IQueryBus
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<CommandBus> _logger = logger;

    public Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query
        , CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing query: {query}", query);
        return _mediator.Send(query, cancellationToken);
    }
}
