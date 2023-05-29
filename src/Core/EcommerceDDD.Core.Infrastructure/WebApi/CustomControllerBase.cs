namespace EcommerceDDD.Core.Infrastructure.WebApi;

public class CustomControllerBase : ControllerBase
{
    private readonly ICommandBus _commandBus;
    private readonly IQueryBus _queryBus;

    public CustomControllerBase() { }

    protected CustomControllerBase(
        ICommandBus commandBus,
        IQueryBus queryBus)
    {
        _commandBus = commandBus;
        _queryBus = queryBus;
    }

    protected async new Task<IActionResult> Response<TResult>(IQuery<TResult> query)
    {
        TResult result;

        try
        {
            result = await _queryBus.Send(query);
        }
        catch (Exception e)
        {
            return BadRequestActionResult(e.Message);
        }

        return Ok(new ApiResponse<TResult>
        {
            Data = result,
            Success = true
        });
    }

    protected async new Task<IActionResult> Response(ICommand command)
    {
        try
        {
            await _commandBus.Send(command);
        }
        catch (Exception e)
        {
            return BadRequestActionResult(e.Message);
        }

        return Ok(new ApiResponse<IActionResult>
        {
            Success = true
        });
    }

    protected IActionResult BadRequestActionResult(string resultErrors)
    {
        return BadRequest(new ApiResponse<IActionResult>
        {
            Success = false,
            Message = resultErrors
        });
    }
}