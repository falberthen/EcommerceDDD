namespace EcommerceDDD.Core.Infrastructure.CQRS;

public class QueryBus(IMediator mediator, ILogger<QueryBus> logger) : IQueryBus
{
	private readonly IMediator _mediator = mediator
		?? throw new ArgumentNullException(nameof(mediator));
	private readonly ILogger<QueryBus> _logger = logger
		?? throw new ArgumentNullException(nameof(logger));

	public Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Executing query: {query}", query);
		return _mediator.Send(query, cancellationToken);
	}
}