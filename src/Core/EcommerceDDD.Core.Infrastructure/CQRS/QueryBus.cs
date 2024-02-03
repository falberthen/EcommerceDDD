namespace EcommerceDDD.Core.Infrastructure.CQRS;

public class QueryBus : IQueryBus
{
    private readonly IMediator _mediator;
    private readonly ILogger<CommandBus> _logger;

    public QueryBus(IMediator mediator, ILogger<CommandBus> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query
        , CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing query: {query}", query);
        return _mediator.Send(query, cancellationToken);
    }
}
