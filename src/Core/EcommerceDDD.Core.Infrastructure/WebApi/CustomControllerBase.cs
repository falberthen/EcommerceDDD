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
            if (!ModelState.IsValid)
                return BadRequest();

            result = await _queryBus.Send(query);
        }
        catch (Exception e)
        {
            return BadRequestActionResult(e.Message);
        }

        return Ok(new
        {
            data = result,
            success = true
        });
    }

    protected async new Task<IActionResult> Response(ICommand command)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _commandBus.Send(command);
        }
        catch (Exception e)
        {
            return BadRequestActionResult(e.Message);
        }

        return Ok(new
        {
            success = true
        });
    }

    private IActionResult BadRequestActionResult(string resultErrors)
    {
        return BadRequest(new
        {
            success = false,
            message = resultErrors
        });
    }
}