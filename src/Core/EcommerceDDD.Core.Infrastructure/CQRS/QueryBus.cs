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

    public Task<TResponse> Send<TResponse>(IQuery<TResponse> query)
    {
        _logger.LogInformation("Executing query: {query}", query);
        return _mediator.Send(query);
    }
}
