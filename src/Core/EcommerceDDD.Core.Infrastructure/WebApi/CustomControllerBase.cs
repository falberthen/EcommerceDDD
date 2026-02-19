namespace EcommerceDDD.Core.Infrastructure.WebApi;

public class CustomControllerBase : ControllerBase
{
    private readonly ICommandBus _commandBus;
    private readonly IQueryBus _queryBus;

	public CustomControllerBase() { }

    protected CustomControllerBase(ICommandBus commandBus, IQueryBus queryBus)
    {
        _commandBus = commandBus
			?? throw new ArgumentNullException(nameof(commandBus));
		_queryBus = queryBus
			?? throw new ArgumentNullException(nameof(queryBus));
	}

    protected async new Task<IActionResult> Response<TResult>(IQuery<TResult> query,
        CancellationToken cancellationToken)
    {
        var result = await _queryBus.SendAsync(query, cancellationToken);
        return Ok(new ApiResponse<TResult>
        {
            Data = result,
            Success = true
        });
    }

    protected async new Task<IActionResult> Response(ICommand command,
		CancellationToken cancellationToken)
    {
        await _commandBus.SendAsync(command, cancellationToken);
        return Ok(new ApiResponse<IActionResult>
        {
            Success = true
        });
    }
}
